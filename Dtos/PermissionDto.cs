using System.ComponentModel.DataAnnotations;

public class PermissionDto
{

    public int Id { get; set; }

    [Required] // This is the missing 'Id' property
    public string Name { get; set; } // Name of the permission
}