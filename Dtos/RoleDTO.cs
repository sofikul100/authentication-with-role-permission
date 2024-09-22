using System.ComponentModel.DataAnnotations;

public class RoleDto
{
    [Required]
    public string Name { get; set; } = default!;
}
