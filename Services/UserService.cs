

using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public interface IUserService
{
    Task<IActionResult> Register(UserRegisterDto dto);
    Task<IActionResult> Login(UserLoginDto dto);
    Task<string> Logout(string token);
}


public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        // Validate input
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);

        if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
            return new BadRequestObjectResult(new { Errors = errors });
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
        {
            return new ConflictObjectResult(new { Error = "Account Already Exits. Please Login." });
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Return user data along with a success message
        var userData = new
        {
            user.Id, // Assuming User has an Id property
            user.Username,
            user.Email
        };

        return new OkObjectResult(new
        {
            Message = "Registration successful!",
            User = userData
        });



    }


    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _context.Users
           .Include(u => u.UserRoles)
               .ThenInclude(ur => ur.Role)
               .ThenInclude(r => r.RolePermissions) // Include permissions for each role
               .ThenInclude(rp => rp.Permission) // Include permission details
           .FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return new BadRequestObjectResult(new { Error = "Username and password are required." });

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
    };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

            // Add permissions for each role
            foreach (var rolePermission in userRole.Role.RolePermissions)
            {
                claims.Add(new Claim("Permission", rolePermission.Permission.Name));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        // Prepare user data to return
        var userData = new
        {
            user.Id,
            user.Username,
            user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            Permissions = user.UserRoles.SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name)).Distinct().ToList() // Get distinct permissions
        };

        // Return the token and the user data
        return new OkObjectResult(new
        {
            Token = jwtToken,
            User = userData
        });
    }




    public async Task<string> Logout(string token)
    {
        // Add token to blacklist
        var tokenBlacklist = new TokenBlacklist
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(30) // Set expiration same as JWT
        };

        _context.TokenBlacklists.Add(tokenBlacklist);
        await _context.SaveChangesAsync();

        return "Logged out successfully.";
    }



}
