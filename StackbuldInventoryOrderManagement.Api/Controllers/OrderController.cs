using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.Order.Dto;
using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Api.Controllers
{
    /// <summary>
    /// Handles order management operations
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/orders")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="request">Order details with products and quantities</param>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _orderService.CreateOrderAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="userId">User ID</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id, [FromQuery] string userId)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _orderService.GetOrderByIdAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        [HttpGet("all")]
        [Authorize(Roles = nameof(UserType.Admin))]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilterDto filter)
        {
            var response = await _orderService.GetAllOrdersAsync(filter);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get current user's orders
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders([FromQuery] OrderFilterDto filter)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _orderService.GetCustomerOrdersAsync(filter);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="id">Order ID</param>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid id, [FromQuery] string userId)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _orderService.CancelOrderAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}