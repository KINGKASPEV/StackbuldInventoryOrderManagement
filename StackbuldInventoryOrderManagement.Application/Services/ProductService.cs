using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackbuldInventoryOrderManagement.Application.Interfaces.Repositories;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.Product.Dto;
using StackbuldInventoryOrderManagement.Application.Product.Model;
using StackbuldInventoryOrderManagement.Common.Responses;
using StackbuldInventoryOrderManagement.Common.Utilities;
using Products = StackbuldInventoryOrderManagement.Domain.Products.Product;

namespace StackbuldInventoryOrderManagement.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Products> _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IGenericRepository<Products> productRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Response<ProductResponse>> CreateProductAsync(CreateProductDto request)
        {
            var response = new Response<ProductResponse>();

            try
            {
                // Check for duplicate SKU if provided
                if (!string.IsNullOrEmpty(request.Sku))
                {
                    var existingProduct = await _productRepository.FindAsync(p => p.Sku == request.Sku);
                    if (existingProduct != null)
                    {
                        response.StatusCode = StatusCodes.Status409Conflict;
                        response.Message = "Product with this SKU already exists.";
                        return response;
                    }
                }

                var product = new Products
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    ImageUrl = request.ImageUrl,
                    Sku = request.Sku,
                    IsActive = true
                };

                await _productRepository.AddAsync(product);
                await _productRepository.SaveAsync();

                var productResponse = MapToProductResponse(product);

                response.StatusCode = StatusCodes.Status201Created;
                response.Data = productResponse;
                response.Message = Constants.SuccessMessage;

                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        public async Task<Response<ProductResponse>> GetProductByIdAsync(Guid id)
        {
            var response = new Response<ProductResponse>();

            try
            {
                var product = await _productRepository.GetByIdAsync2(id);

                if (product == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Product not found.";
                    return response;
                }

                var productResponse = MapToProductResponse(product);

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = productResponse;
                response.Message = Constants.SuccessMessage;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        public async Task<Response<PagedResult<ProductResponse>>> GetAllProductsAsync(ProductFilterDto filter)
        {
            var response = new Response<PagedResult<ProductResponse>>();

            try
            {
                var pagedResult = await _productRepository.GetPagedAsync(
                    filter.Page,
                    filter.PageSize,
                    filter: p =>
                        (string.IsNullOrEmpty(filter.SearchTerm) ||
                         p.Name.Contains(filter.SearchTerm) ||
                         p.Description.Contains(filter.SearchTerm) ||
                         (p.Sku != null && p.Sku.Contains(filter.SearchTerm))) &&
                        (!filter.MinPrice.HasValue || p.Price >= filter.MinPrice.Value) &&
                        (!filter.MaxPrice.HasValue || p.Price <= filter.MaxPrice.Value) &&
                        (!filter.IsActive.HasValue || p.IsActive == filter.IsActive.Value),
                    orderBy: q => q.OrderByDescending(p => p.DateCreated)
                );

                var mappedResult = new PagedResult<ProductResponse>
                {
                    CurrentPage = pagedResult.CurrentPage,
                    PageSize = pagedResult.PageSize,
                    TotalCount = pagedResult.TotalCount,
                    TotalPages = pagedResult.TotalPages,
                    Items = pagedResult.Items.Select(MapToProductResponse).ToList()
                };

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = mappedResult;
                response.Message = Constants.SuccessMessage;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        public async Task<Response<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductDto request)
        {
            var response = new Response<ProductResponse>();

            try
            {
                var product = await _productRepository.GetByIdAsync2(id);

                if (product == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Product not found.";
                    return response;
                }

                // Check for duplicate SKU if changed
                if (!string.IsNullOrEmpty(request.Sku) && request.Sku != product.Sku)
                {
                    var existingProduct = await _productRepository.FindAsync(p => p.Sku == request.Sku);
                    if (existingProduct != null)
                    {
                        response.StatusCode = StatusCodes.Status409Conflict;
                        response.Message = "Product with this SKU already exists.";
                        return response;
                    }
                }

                if (!string.IsNullOrEmpty(request.Name)) product.Name = request.Name;
                if (!string.IsNullOrEmpty(request.Description)) product.Description = request.Description;
                if (request.Price.HasValue) product.Price = request.Price.Value;
                if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity.Value;
                if (!string.IsNullOrEmpty(request.ImageUrl)) product.ImageUrl = request.ImageUrl;
                if (!string.IsNullOrEmpty(request.Sku)) product.Sku = request.Sku;
                if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveAsync();

                var productResponse = MapToProductResponse(product);

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = productResponse;
                response.Message = Constants.SuccessMessage;

                _logger.LogInformation("Product updated successfully with ID: {ProductId}", product.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        public async Task<Response<bool>> DeleteProductAsync(Guid id)
        {
            var response = new Response<bool>();

            try
            {
                var product = await _productRepository.GetByIdAsync2(id);

                if (product == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Product not found.";
                    return response;
                }

                // Soft delete by setting IsActive to false
                product.IsActive = false;
                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveAsync();

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = true;
                response.Message = "Product deleted successfully.";

                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        public async Task<Response<bool>> CheckProductAvailabilityAsync(Guid productId, int quantity)
        {
            var response = new Response<bool>();

            try
            {
                var product = await _productRepository.GetByIdAsync2(productId);

                if (product == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Product not found.";
                    response.Data = false;
                    return response;
                }

                if (!product.IsActive)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Product is not active.";
                    response.Data = false;
                    return response;
                }

                if (product.StockQuantity < quantity)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = $"Insufficient stock. Available: {product.StockQuantity}, Requested: {quantity}";
                    response.Data = false;
                    return response;
                }

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = true;
                response.Message = "Product is available.";

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking product availability for ID: {ProductId}", productId);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        private ProductResponse MapToProductResponse(Products product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                Sku = product.Sku,
                DateCreated = product.DateCreated,
                DateModified = product.DateModified
            };
        }
    }
}
