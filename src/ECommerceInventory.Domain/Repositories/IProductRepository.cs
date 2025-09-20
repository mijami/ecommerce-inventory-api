using ECommerceInventory.Domain.Entities;

namespace ECommerceInventory.Domain.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> SearchByNameOrDescriptionAsync(string searchTerm);
    Task<IEnumerable<Product>> GetPaginatedAsync(int page, int limit);
    Task<IEnumerable<Product>> GetFilteredAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, int page, int limit);
    Task<Product?> GetByIdWithCategoryAsync(int id);
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
}
