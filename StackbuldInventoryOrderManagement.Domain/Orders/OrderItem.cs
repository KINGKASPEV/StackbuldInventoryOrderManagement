using StackbuldInventoryOrderManagement.Domain.Common;
using StackbuldInventoryOrderManagement.Domain.Products;

namespace StackbuldInventoryOrderManagement.Domain.Orders
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
