public class RoleWithPermissionsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PermissionDto> Permissions { get; set; }
}