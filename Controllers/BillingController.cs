using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopBillingSystem.DTOs;
using ShopBillingSystem.Services;
using ShopBillingSystem.ViewModels;
using System.Security.Claims;

namespace ShopBillingSystem.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;
        private readonly IProductService _productService;

        public BillingController(IBillingService billingService, IProductService productService)
        {
            _billingService = billingService;
            _productService = productService;
        }

        // GET: Billing
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAvailableProductsAsync();
            var selectList = products.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} - ${p.Price:F2} (Stock: {p.StockQuantity})"
            }).ToList();

            ViewBag.Products = selectList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            var result = await _billingService.GetProductDetailsAsync(productId);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessBill([FromBody] BillRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new BillResponseDto
                {
                    Success = false,
                    Message = "Invalid request data"
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var result = await _billingService.ProcessBillAsync(request, userId);
            
            return Json(result);
        }

        // GET: Billing/Receipt/5
        public async Task<IActionResult> Receipt(int id)
        {
            var order = await _billingService.GetOrderWithDetailsAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Security: Verify the order belongs to the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var viewModel = MapToOrderViewModel(order);
            return View(viewModel);
        }

        // GET: Billing/Orders
        public async Task<IActionResult> Orders()
        {
            // Security: Only show orders for the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _billingService.GetOrdersByUserIdAsync(userId ?? string.Empty);
            var viewModels = orders.Select(MapToOrderViewModel).ToList();
            
            var listViewModel = new OrderListViewModel
            {
                Orders = viewModels
            };

            return View(listViewModel);
        }

        private static OrderViewModel MapToOrderViewModel(OrderDto dto)
        {
            return new OrderViewModel
            {
                Id = dto.Id,
                CustomerName = dto.CustomerName,
                OrderDate = dto.OrderDate,
                TotalAmount = dto.TotalAmount,
                UserId = dto.UserId,
                OrderItems = dto.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
