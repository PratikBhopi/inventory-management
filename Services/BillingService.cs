using ShopBillingSystem.DTOs;
using ShopBillingSystem.Models;
using ShopBillingSystem.Repositories;

namespace ShopBillingSystem.Services
{
    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BillingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductDetailsDto> GetProductDetailsAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            
            if (product == null)
            {
                return new ProductDetailsDto
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            return new ProductDetailsDto
            {
                Success = true,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }

        public async Task<BillResponseDto> ProcessBillAsync(BillRequestDto request, string userId)
        {
            // Validation
            if (request?.Items == null || !request.Items.Any())
            {
                return new BillResponseDto
                {
                    Success = false,
                    Message = "No items in the bill"
                };
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return new BillResponseDto
                {
                    Success = false,
                    Message = "Customer name is required"
                };
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Validate stock availability and pair items with products
                var validationErrors = new List<string>();
                var validatedItems = new List<(BillItemDto item, Product product)>();

                foreach (var item in request.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        validationErrors.Add($"Product with ID {item.ProductId} not found");
                        continue;
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        validationErrors.Add($"Insufficient stock for {product.Name}. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                        continue;
                    }

                    validatedItems.Add((item, product));
                }

                if (validationErrors.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return new BillResponseDto
                    {
                        Success = false,
                        Message = string.Join("; ", validationErrors)
                    };
                }

                // Create the order
                var order = new Order
                {
                    CustomerName = request.CustomerName,
                    OrderDate = DateTime.Now,
                    UserId = userId,
                    TotalAmount = 0
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync(); // Save to get OrderId

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                // Create order items and update inventory
                foreach (var (item, product) in validatedItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    orderItems.Add(orderItem);
                    totalAmount += orderItem.Subtotal;

                    // Update inventory
                    await _unitOfWork.Products.UpdateStockAsync(item.ProductId, item.Quantity);
                }

                // Update order total
                order.TotalAmount = totalAmount;
                _unitOfWork.Orders.Update(order);

                // Add all order items
                await _unitOfWork.OrderItems.AddRangeAsync(orderItems);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new BillResponseDto
                {
                    Success = true,
                    Message = "Bill generated successfully",
                    OrderId = order.Id,
                    TotalAmount = totalAmount
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new BillResponseDto
                {
                    Success = false,
                    Message = $"Error processing bill: {ex.Message}"
                };
            }
        }

        public async Task<OrderDto?> GetOrderWithDetailsAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
            return order == null ? null : MapToOrderDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetOrdersWithItemsAsync();
            return orders.Select(MapToOrderDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapToOrderDto);
        }

        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
