using Microsoft.EntityFrameworkCore;
using ShopBillingSystem.Data;
using ShopBillingSystem.Models;

namespace ShopBillingSystem.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await _dbSet
                .Where(p => p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(p => p.Category == category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            return product != null && product.StockQuantity >= quantity;
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                product.StockQuantity -= quantity;
                Update(product);
            }
        }
    }
}
