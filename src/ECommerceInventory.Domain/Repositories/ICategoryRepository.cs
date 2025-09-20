using ECommerceInventory.Domain.Entities;

namespace ECommerceInventory.Domain.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<bool> NameExistsAsync(string name);
    Task<bool> HasProductsAsync(int categoryId);
    Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync();
}
