using ShopBillingSystem.Models;

namespace ShopBillingSystem.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
        Task<bool> IsStockAvailableAsync(int productId, int quantity);
        Task UpdateStockAsync(int productId, int quantity);
    }
}
