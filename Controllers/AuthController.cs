using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)

    {

        var result = await _userService.Register(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {

        var result = await _userService.Login(dto);

        if (result == null)
            return Unauthorized("Invalid credentials");

        return Ok(result); // Return token and user data
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Extract the token from the Authorization header
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token is required for logout.");
        }

        var result = await _userService.Logout(token);
        return Ok(result);
    }
}
