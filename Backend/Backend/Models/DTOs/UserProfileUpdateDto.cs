using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs;

public class UserProfileUpdateDto
{
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }
} 