using StackbuldInventoryOrderManagement.Application.Product.Dto;
using StackbuldInventoryOrderManagement.Application.Product.Model;
using StackbuldInventoryOrderManagement.Common.Responses;
using StackbuldInventoryOrderManagement.Common.Utilities;

namespace StackbuldInventoryOrderManagement.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Response<ProductResponse>> CreateProductAsync(CreateProductDto request);
        Task<Response<ProductResponse>> GetProductByIdAsync(Guid id);
        Task<Response<PagedResult<ProductResponse>>> GetAllProductsAsync(ProductFilterDto filter);
        Task<Response<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductDto request);
        Task<Response<bool>> DeleteProductAsync(Guid id);
        Task<Response<bool>> CheckProductAvailabilityAsync(Guid productId, int quantity);
    }
}
