using ECommerceInventory.Application.DTOs;
using ECommerceInventory.Application.Exceptions;
using ECommerceInventory.Domain.Entities;
using ECommerceInventory.Domain.Repositories;

namespace ECommerceInventory.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetCategoriesWithProductCountAsync();

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            IsActive = c.IsActive,
            ProductCount = c.Products.Count,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
            return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = category.Products.Count,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        // Check if category name already exists
        if (await _unitOfWork.Categories.NameExistsAsync(createCategoryDto.Name))
        {
            throw new ConflictException("Category name already exists");
        }

        var category = new Category
        {
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            IsActive = true
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = 0,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        // Check if the new name already exists (excluding current category)
        var existingCategory = await _unitOfWork.Categories.GetByNameAsync(updateCategoryDto.Name);
        if (existingCategory != null && existingCategory.Id != id)
        {
            throw new ConflictException("Category name already exists");
        }

        category.Name = updateCategoryDto.Name;
        category.Description = updateCategoryDto.Description;
        category.IsActive = updateCategoryDto.IsActive;

        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = category.Products.Count,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return false;
        }

        // Check if category has linked products
        if (await _unitOfWork.Categories.HasProductsAsync(id))
        {
            throw new ConflictException("Cannot delete category with linked products");
        }

        await _unitOfWork.Categories.DeleteAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}