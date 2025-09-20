using Microsoft.EntityFrameworkCore;
using ECommerceInventory.Domain.Entities;
using ECommerceInventory.Domain.Repositories;
using ECommerceInventory.Infrastructure.Data;

namespace ECommerceInventory.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.IsActive)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNameOrDescriptionAsync(string searchTerm)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => (p.Name.Contains(searchTerm) ||
                        (p.Description != null && p.Description.Contains(searchTerm))) &&
                        p.IsActive)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetPaginatedAsync(int page, int limit)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFilteredAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int limit)
    {
        IQueryable<Product> query = _dbSet.Include(p => p.Category).Where(p => p.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        query = query.OrderBy(p => p.Id);

        return await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdWithCategoryAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await GetAllWithCategoryAsync();
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await GetByIdWithCategoryAsync(id);
    }
}