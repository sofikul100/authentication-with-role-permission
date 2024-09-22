using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class UserRegisterDto
{

    [Required]
    public string Username { get; set; } = default !;

    [Required]
    [EmailAddress()]
    
    public string Email { get; set; } = default !;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } =default!;
}
