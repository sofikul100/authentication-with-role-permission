public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = default !;

    public ICollection<RolePermission> RolePermissions { get; set; } = default !;
}
