using ShopBillingSystem.DTOs;

namespace ShopBillingSystem.Services
{
    public interface IBillingService
    {
        Task<ProductDetailsDto> GetProductDetailsAsync(int productId);
        Task<BillResponseDto> ProcessBillAsync(BillRequestDto request, string userId);
        Task<OrderDto?> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
    }
}
