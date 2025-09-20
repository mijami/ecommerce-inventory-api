using Microsoft.EntityFrameworkCore;
using ECommerceInventory.Domain.Entities;
using ECommerceInventory.Domain.Repositories;
using ECommerceInventory.Infrastructure.Data;

namespace ECommerceInventory.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _dbSet.AnyAsync(c => c.Name == name);
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync()
    {
        return await _dbSet
            .Include(c => c.Products)
            .Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Products = c.Products
            })
            .ToListAsync();
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet.Include(c => c.Products).ToListAsync();
    }

    public override async Task<Category?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
    }
}
