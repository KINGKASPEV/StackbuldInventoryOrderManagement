namespace StackbuldInventoryOrderManagement.Application.Order.Dto
{
    public class CreateOrderDto
    {
        public string CustomerId { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
