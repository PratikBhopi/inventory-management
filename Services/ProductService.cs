using ShopBillingSystem.DTOs;
using ShopBillingSystem.Models;
using ShopBillingSystem.Repositories;

namespace ShopBillingSystem.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAvailableProductsAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            var product = new Product
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                StockQuantity = createDto.StockQuantity,
                Category = createDto.Category,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(UpdateProductDto updateDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(updateDto.Id);
            if (product == null)
                return null;

            product.Name = updateDto.Name;
            product.Description = updateDto.Description;
            product.Price = updateDto.Price;
            product.StockQuantity = updateDto.StockQuantity;
            product.Category = updateDto.Category;
            // CreatedDate is immutable - not updated

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return false;

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _unitOfWork.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(category);
            return products.Select(MapToDto);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Category = product.Category,
                CreatedDate = product.CreatedDate
            };
        }
    }
}
