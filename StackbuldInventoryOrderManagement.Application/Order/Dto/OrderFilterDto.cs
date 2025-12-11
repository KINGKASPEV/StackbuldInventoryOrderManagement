using StackbuldInventoryOrderManagement.Domain.Orders;

namespace StackbuldInventoryOrderManagement.Application.Order.Dto
{
    public class OrderFilterDto
    {
        public string? CustomerId { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
