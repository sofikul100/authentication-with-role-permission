using Microsoft.EntityFrameworkCore;

public interface IPermissionService
{
    Task<IEnumerable<Permission>> GetAllPermissionsAsync();
    Task<Permission> GetPermissionByIdAsync(int id);
    Task<Permission> CreatePermissionAsync(PermissionDto permissionDto);
    Task<Permission> UpdatePermissionAsync(int id, PermissionDto permissionDto);

    Task DeletePermissionAsync(int id);
    Task AssignPermissionsToRoleAsync(RolePermissionDto rolePermissionDto);
    Task<RoleWithPermissionsDto> GetRoleWithPermissionsAsync(int roleId);
}

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;

    public PermissionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
    {
        return await _context.Permissions.ToListAsync();
    }

    public async Task<Permission> GetPermissionByIdAsync(int id)
    {
        return await _context.Permissions.FindAsync(id) ?? throw new KeyNotFoundException("Permission not found");
    }

    public async Task<Permission> CreatePermissionAsync(PermissionDto permissionDto)
    {
        var permission = new Permission { Name = permissionDto.Name };
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();
        return permission;
    }

    public async Task<Permission> UpdatePermissionAsync(int id, PermissionDto permissionDto)
    {
        var permission = await GetPermissionByIdAsync(id);  // Fetch the permission by ID
        permission.Name = permissionDto.Name;  // Update the name

        await _context.SaveChangesAsync();  // Save changes to the database
        return permission;
    }

    public async Task DeletePermissionAsync(int id)
    {
        var permission = await GetPermissionByIdAsync(id);
        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();
    }

    public async Task AssignPermissionsToRoleAsync(RolePermissionDto rolePermissionDto)
    {
        // Ensure the role exists
        var role = await _context.Roles.FindAsync(rolePermissionDto.RoleId);
        if (role == null)
            throw new KeyNotFoundException("Role not found");

        // Remove existing permissions for this role
        var existingRolePermissions = _context.RolePermissions
            .Where(rp => rp.RoleId == rolePermissionDto.RoleId);
        _context.RolePermissions.RemoveRange(existingRolePermissions);

        // Assign new permissions
        var newRolePermissions = rolePermissionDto.PermissionIds.Select(permissionId => new RolePermission
        {
            RoleId = rolePermissionDto.RoleId,
            PermissionId = permissionId
        }).ToList();

        await _context.RolePermissions.AddRangeAsync(newRolePermissions);
        await _context.SaveChangesAsync();
    }


    public async Task<RoleWithPermissionsDto> GetRoleWithPermissionsAsync(int roleId)
    {
        // Retrieve the role and its associated permissions
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);

        if (role == null)
            throw new KeyNotFoundException("Role not found");

        // Map to DTO to avoid circular references and expose necessary details
        var roleWithPermissionsDto = new RoleWithPermissionsDto
        {
            Id = role.Id,
            Name = role.Name,
            Permissions = role.RolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name
            }).ToList()
        };

        return roleWithPermissionsDto;
    }


















}

