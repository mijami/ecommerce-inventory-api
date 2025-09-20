using System.ComponentModel.DataAnnotations;

namespace ECommerceInventory.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageBase64 { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public CategoryDto? Category { get; set; }
}

public class CreateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageBase64 { get; set; }
}

public class UpdateProductDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageBase64 { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProductQueryDto
{
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 10;
}
