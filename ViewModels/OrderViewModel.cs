using System.ComponentModel.DataAnnotations;

namespace ShopBillingSystem.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        public string? UserId { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new();

        [Display(Name = "Items Count")]
        public int ItemsCount => OrderItems.Sum(oi => oi.Quantity);
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product")]
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Display(Name = "Unit Price")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal => Quantity * UnitPrice;
    }

    public class OrderListViewModel
    {
        public List<OrderViewModel> Orders { get; set; } = new();
        public int TotalOrders => Orders.Count;
        public decimal TotalRevenue => Orders.Sum(o => o.TotalAmount);
        public int TotalItemsSold => Orders.Sum(o => o.ItemsCount);
        public decimal AverageOrderValue => TotalOrders > 0 ? TotalRevenue / TotalOrders : 0;
    }
}
