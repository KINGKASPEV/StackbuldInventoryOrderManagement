using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.Product.Dto;
using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Api.Controllers
{
    /// <summary>
    /// Handles product catalog operations
    /// </summary>
    [Route("api/products")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserType.Admin))]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Create a new product (Admin only)
        /// </summary>
        /// <param name="request">Product creation details</param>
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _productService.CreateProductAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get a product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get all products with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilterDto filter)
        {
            var response = await _productService.GetAllProductsAsync(filter);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Update a product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="request">Updated product details</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _productService.UpdateProductAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Delete a product (Admin only)
        /// </summary>
        /// <param name="id">Product ID</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var response = await _productService.DeleteProductAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Check product availability
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">Desired quantity</param>
        [HttpGet("availability")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAvailability([FromQuery] Guid productId, [FromQuery] int quantity)
        {
            var response = await _productService.CheckProductAvailabilityAsync(productId, quantity);
            return StatusCode(response.StatusCode, response);
        }
    }
}