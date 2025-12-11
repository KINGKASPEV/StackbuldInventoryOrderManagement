namespace StackbuldInventoryOrderManagement.Application.Order.Dto
{
    public class CreateOrderDto
    {
        public string? ShippingAddress { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
