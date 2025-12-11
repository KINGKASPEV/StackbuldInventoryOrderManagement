using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.Order.Dto;
using System.Security.Claims;

namespace StackbuldInventoryOrderManagement.Api.Controllers
{
    /// <summary>
    /// Handles order management operations
    /// </summary>
    [Route("api/orders")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpPost]
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);

            // Ensure users can only view their own orders (unless admin)
            if (response.StatusCode == StatusCodes.Status200OK && response.Data != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && response.Data.CustomerId != userId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "Access denied");
                }
            }

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get all orders (Admin only)
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            // First check if the order exists and belongs to the user
            var orderResponse = await _orderService.GetOrderByIdAsync(id);

            if (orderResponse.StatusCode == StatusCodes.Status404NotFound)
                return StatusCode(orderResponse.StatusCode, orderResponse);

            if (orderResponse.Data != null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && orderResponse.Data.CustomerId != userId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "Access denied");
                }
            }

            var response = await _orderService.CancelOrderAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}