using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Username Is Required")]
    public string Username { get; set; } = default!;
    [Required(ErrorMessage = "Email Is Required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Password Is Required")]
    [MinLength(8, ErrorMessage = "Password should be at least 8 characters")]
    public string PasswordHash { get; set; } = default!;

    public ICollection<UserRole> UserRoles { get; set; } = default!;
}
