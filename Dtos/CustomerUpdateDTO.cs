using System;
using System.ComponentModel.DataAnnotations;

public class CustomerUpdateDTO
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default !;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = default !;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; } = default !;

    public DateTime? UpdatedAt { get; set; }
}