using Microsoft.EntityFrameworkCore;
using ShopBillingSystem.Data;
using ShopBillingSystem.Models;

namespace ShopBillingSystem.Repositories
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _dbSet
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByProductIdAsync(int productId)
        {
            return await _dbSet
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == productId)
                .ToListAsync();
        }
    }
}
