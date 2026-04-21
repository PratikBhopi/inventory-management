using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopBillingSystem.DTOs;
using ShopBillingSystem.Services;
using ShopBillingSystem.ViewModels;

namespace ShopBillingSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var viewModels = products.Select(MapToViewModel);
            return View(viewModels);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(MapToViewModel(product));
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            return View(new CreateProductViewModel());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var createDto = new CreateProductDto
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    StockQuantity = viewModel.StockQuantity,
                    Category = viewModel.Category
                };

                await _productService.CreateProductAsync(createDto);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Category = product.Category,
                CreatedDate = product.CreatedDate
            };

            return View(viewModel);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, EditProductViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updateDto = new UpdateProductDto
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    StockQuantity = viewModel.StockQuantity,
                    Category = viewModel.Category
                };

                var result = await _productService.UpdateProductAsync(updateDto);
                if (result == null)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Products/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(MapToViewModel(product));
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private static ProductViewModel MapToViewModel(ProductDto dto)
        {
            return new ProductViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Category = dto.Category,
                CreatedDate = dto.CreatedDate
            };
        }
    }
}
