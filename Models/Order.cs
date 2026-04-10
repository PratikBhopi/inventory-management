using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopBillingSystem.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [StringLength(450)]
        public string? UserId { get; set; }

        // Navigation property: One Order has many OrderItems
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
