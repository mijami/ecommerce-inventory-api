using System.ComponentModel.DataAnnotations;
using ECommerceInventory.Domain.Common;

namespace ECommerceInventory.Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
