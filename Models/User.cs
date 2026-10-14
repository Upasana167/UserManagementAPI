using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required and cannot contain only whitespace.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;
}