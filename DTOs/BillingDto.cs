using System.ComponentModel.DataAnnotations;

namespace ShopBillingSystem.DTOs
{
    public class BillRequestDto
    {
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<BillItemDto> Items { get; set; } = new();
    }

    public class BillItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class BillResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class ProductDetailsDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
