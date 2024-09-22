using Microsoft.EntityFrameworkCore;
public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<Role> GetRoleByIdAsync(int id);
    Task<Role> CreateRoleAsync(RoleDto roleDto);
    Task<Role> UpdateRoleAsync(int id, RoleDto roleDto);
    Task DeleteRoleAsync(int id);
    Task AssignRoleToUser(int userId, int roleId);

}


public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role> GetRoleByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id) ?? throw new KeyNotFoundException("Role not found");
    }

    public async Task<Role> CreateRoleAsync(RoleDto roleDto)
    {
        var role = new Role { Name = roleDto.Name };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<Role> UpdateRoleAsync(int id, RoleDto roleDto)
    {
        var role = await GetRoleByIdAsync(id);
        role.Name = roleDto.Name;
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task DeleteRoleAsync(int id)
    {
        var role = await GetRoleByIdAsync(id);
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
    }

    public async Task AssignRoleToUser(int userId, int roleId)
    {
        // Check if the user already has a role
        var existingUserRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId);

        if (existingUserRole != null)
        {
            // Remove the existing role
            _context.UserRoles.Remove(existingUserRole);
            await _context.SaveChangesAsync();
        }

        // Assign the new role
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }





}
