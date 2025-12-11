using StackbuldInventoryOrderManagement.Domain.Common;
using StackbuldInventoryOrderManagement.Domain.Orders;

namespace StackbuldInventoryOrderManagement.Domain.Products
{
    public class Product : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
        public string? Sku { get; set; } // Stock Keeping Unit

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
