using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;


    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;

    }

    [HttpGet]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _permissionService.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPermissionById(int id)
    {
        var permission = await _permissionService.GetPermissionByIdAsync(id);
        return Ok(permission);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePermission(PermissionDto permissionDto)
    {
        var permission = await _permissionService.CreatePermissionAsync(permissionDto);
        return CreatedAtAction(nameof(GetPermissionById), new { id = permission.Name }, permission);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePermission(int id, PermissionDto permissionDto)
    {
        var updatedPermission = await _permissionService.UpdatePermissionAsync(id, permissionDto);
        return Ok(updatedPermission);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermission(int id)
    {
        await _permissionService.DeletePermissionAsync(id);
        return NoContent();
    }


    [HttpPost("assign-permissions")]
    public async Task<IActionResult> AssignPermissionsToRole(RolePermissionDto rolePermissionDto)
    {
        await _permissionService.AssignPermissionsToRoleAsync(rolePermissionDto);
        return Ok("Permissions assigned successfully.");
    }

    [HttpGet("{roleId}/permissions")]
    public async Task<IActionResult> GetRoleWithPermissions(int roleId)
    {
        var roleWithPermissions = await _permissionService.GetRoleWithPermissionsAsync(roleId);
        return Ok(roleWithPermissions);
    }




}
