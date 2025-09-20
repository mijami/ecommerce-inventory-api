using ECommerceInventory.Application.DTOs;
using ECommerceInventory.Domain.Entities;
using ECommerceInventory.Domain.Repositories;

namespace ECommerceInventory.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllWithCategoryAsync();
        return MapToProductDtos(products);
    }

    public async Task<IEnumerable<ProductDto>> GetFilteredProductsAsync(ProductQueryDto query)
    {
        var products = await _unitOfWork.Products.GetFilteredAsync(
            query.CategoryId, 
            query.MinPrice, 
            query.MaxPrice, 
            query.Page, 
            query.Limit);

        return MapToProductDtos(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.SearchByNameOrDescriptionAsync(searchTerm);
        return MapToProductDtos(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);
        
        if (product == null)
            return null;

        return MapToProductDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        // Validate category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(createProductDto.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            Stock = createProductDto.Stock,
            CategoryId = createProductDto.CategoryId,
            ImageUrl = createProductDto.ImageUrl,
            ImageBase64 = createProductDto.ImageBase64,
            IsActive = true
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        // Fetch the product with category to return complete DTO
        var createdProduct = await _unitOfWork.Products.GetByIdWithCategoryAsync(product.Id);
        return MapToProductDto(createdProduct!);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        // Validate category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(updateProductDto.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        product.Name = updateProductDto.Name;
        product.Description = updateProductDto.Description;
        product.Price = updateProductDto.Price;
        product.Stock = updateProductDto.Stock;
        product.CategoryId = updateProductDto.CategoryId;
        product.ImageUrl = updateProductDto.ImageUrl;
        product.ImageBase64 = updateProductDto.ImageBase64;
        product.IsActive = updateProductDto.IsActive;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        // Fetch the updated product with category to return complete DTO
        var updatedProduct = await _unitOfWork.Products.GetByIdWithCategoryAsync(product.Id);
        return MapToProductDto(updatedProduct!);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        
        if (product == null)
        {
            return false;
        }

        await _unitOfWork.Products.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ProductDto MapToProductDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            ImageUrl = product.ImageUrl,
            ImageBase64 = product.ImageBase64,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Category = product.Category != null ? new CategoryDto
            {
                Id = product.Category.Id,
                Name = product.Category.Name,
                Description = product.Category.Description,
                IsActive = product.Category.IsActive,
                CreatedAt = product.Category.CreatedAt,
                UpdatedAt = product.Category.UpdatedAt
            } : null
        };
    }

    private static IEnumerable<ProductDto> MapToProductDtos(IEnumerable<Product> products)
    {
        return products.Select(MapToProductDto);
    }
}
