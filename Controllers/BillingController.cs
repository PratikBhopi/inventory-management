using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopBillingSystem.Data;
using ShopBillingSystem.Models;
using System.Security.Claims;

namespace ShopBillingSystem.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Billing
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.StockQuantity > 0)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} - ${p.Price:F2} (Stock: {p.StockQuantity})"
                })
                .ToListAsync();

            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            return Json(new
            {
                success = true,
                id = product.Id,
                name = product.Name,
                price = product.Price,
                stockQuantity = product.StockQuantity
            });
        }

        [HttpPost]
        public async Task<IActionResult> ProcessBill([FromBody] BillRequest request)
        {
            if (request?.Items == null || !request.Items.Any())
            {
                return Json(new { success = false, message = "No items in the bill" });
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return Json(new { success = false, message = "Customer name is required" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate stock availability for all items
                var validationErrors = new List<string>();
                var productsToUpdate = new List<Product>();

                foreach (var item in request.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
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

                    productsToUpdate.Add(product);
                }

                if (validationErrors.Any())
                {
                    return Json(new { success = false, message = string.Join("; ", validationErrors) });
                }

                // Create the order
                var order = new Order
                {
                    CustomerName = request.CustomerName,
                    OrderDate = DateTime.Now,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    TotalAmount = 0 // Will be calculated below
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Save to get the OrderId

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                // Create order items and update inventory
                for (int i = 0; i < request.Items.Count; i++)
                {
                    var item = request.Items[i];
                    var product = productsToUpdate[i];

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
                    product.StockQuantity -= item.Quantity;
                    _context.Products.Update(product);
                }

                // Update order total
                order.TotalAmount = totalAmount;
                _context.Orders.Update(order);

                // Add all order items
                _context.OrderItems.AddRange(orderItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Bill generated successfully",
                    orderId = order.Id,
                    totalAmount = totalAmount.ToString("F2")
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = $"Error processing bill: {ex.Message}" });
            }
        }

        // GET: Billing/Receipt/5
        public async Task<IActionResult> Receipt(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Billing/Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }

    public class BillRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public List<BillItem> Items { get; set; } = new();
    }

    public class BillItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}