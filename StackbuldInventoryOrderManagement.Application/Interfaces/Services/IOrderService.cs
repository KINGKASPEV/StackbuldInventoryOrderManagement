using StackbuldInventoryOrderManagement.Application.Order.Dto;
using StackbuldInventoryOrderManagement.Application.Order.Model;
using StackbuldInventoryOrderManagement.Common.Responses;
using StackbuldInventoryOrderManagement.Common.Utilities;

namespace StackbuldInventoryOrderManagement.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Response<OrderResponse>> CreateOrderAsync(CreateOrderDto request);
        Task<Response<OrderResponse>> GetOrderByIdAsync(Guid id);
        Task<Response<PagedResult<OrderResponse>>> GetAllOrdersAsync(OrderFilterDto filter);
        Task<Response<PagedResult<OrderResponse>>> GetCustomerOrdersAsync(OrderFilterDto filter);
        Task<Response<bool>> CancelOrderAsync(Guid id);
    }
}
