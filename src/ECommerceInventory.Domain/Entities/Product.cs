using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerceInventory.Domain.Common;

namespace ECommerceInventory.Domain.Entities;

public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    // Optional image fields (for bonus feature)
    public string? ImageUrl { get; set; }
    
    public string? ImageBase64 { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual Category Category { get; set; } = null!;
}
