using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackbuldInventoryOrderManagement.Application.Interfaces.Persistence;
using StackbuldInventoryOrderManagement.Application.Interfaces.Repositories;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.Order.Dto;
using StackbuldInventoryOrderManagement.Application.Order.Model;
using StackbuldInventoryOrderManagement.Common.Responses;
using StackbuldInventoryOrderManagement.Common.Utilities;
using StackbuldInventoryOrderManagement.Domain.Orders;
using StackbuldInventoryOrderManagement.Domain.Users;
using Orders = StackbuldInventoryOrderManagement.Domain.Orders.Order;
using Products = StackbuldInventoryOrderManagement.Domain.Products.Product;

namespace StackbuldInventoryOrderManagement.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Orders> _orderRepository;
        private readonly IGenericRepository<Products> _productRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IGenericRepository<Orders> orderRepository,
            IGenericRepository<Products> productRepository,
            UserManager<ApplicationUser> userManager,
            IGenericRepository<OrderItem> orderItemRepository,
            IAppDbContext dbContext,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        //public async Task<Response<OrderResponse>> CreateOrderAsync(CreateOrderDto request)
        //{
        //    var response = new Response<OrderResponse>();

        //    var customer = await _userManager.FindByIdAsync(request.CustomerId);
        //    if (customer is null)
        //    {
        //        _logger.LogWarning("Customer not found with ID {UserId}", request.CustomerId);
        //        response.StatusCode = StatusCodes.Status404NotFound;
        //        response.Message = Constants.CustomerNotFound;
        //        return response;
        //    }

        //    using var transaction = await _dbContext.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var productIds = request.OrderItems.Select(x => x.ProductId).ToList();

        //        var products = await _productRepository
        //            .FindByCondition(p => productIds.Contains(p.Id), trackChanges: true)
        //            .ToListAsync();

        //        // This prevents other transactions from modifying these rows until we commit

        //        if (products.Count != productIds.Distinct().Count())
        //        {
        //            response.StatusCode = StatusCodes.Status404NotFound;
        //            response.Message = "One or more products not found.";
        //            return response;
        //        }

        //        // Validate stock availability and active status
        //        var insufficientStockItems = new List<string>();
        //        var inactiveProducts = new List<string>();

        //        foreach (var orderItem in request.OrderItems)
        //        {
        //            var product = products.FirstOrDefault(p => p.Id == orderItem.ProductId);

        //            if (product == null)
        //            {
        //                insufficientStockItems.Add($"Product not found");
        //                continue;
        //            }

        //            if (!product.IsActive)
        //            {
        //                inactiveProducts.Add($"{product.Name} is not available");
        //            }

        //            if (product.StockQuantity < orderItem.Quantity)
        //            {
        //                insufficientStockItems.Add(
        //                    $"{product.Name}: Requested {orderItem.Quantity}, Available {product.StockQuantity}"
        //                );
        //            }
        //        }

        //        if (inactiveProducts.Any())
        //        {
        //            response.StatusCode = StatusCodes.Status400BadRequest;
        //            response.Message = $"Inactive products: {string.Join(", ", inactiveProducts)}";
        //            return response;
        //        }

        //        if (insufficientStockItems.Any())
        //        {
        //            response.StatusCode = StatusCodes.Status400BadRequest;
        //            response.Message = $"Insufficient stock: {string.Join("; ", insufficientStockItems)}";
        //            return response;
        //        }

        //        var order = new Orders
        //        {
        //            OrderNumber = GenerateOrderNumber(),
        //            CustomerId = request.CustomerId,
        //            Status = OrderStatus.Pending,
        //            ShippingAddress = request.ShippingAddress,
        //            Notes = request.Notes,
        //            TotalAmount = 0
        //        };

        //        await _orderRepository.AddAsync(order);

        //        decimal totalAmount = 0;
        //        foreach (var itemDto in request.OrderItems)
        //        {
        //            var product = products.First(p => p.Id == itemDto.ProductId);

        //            var orderItem = new OrderItem
        //            {
        //                OrderId = order.Id,
        //                ProductId = product.Id,
        //                Quantity = itemDto.Quantity,
        //                UnitPrice = product.Price,
        //                TotalPrice = product.Price * itemDto.Quantity
        //            };

        //            await _orderItemRepository.AddAsync(orderItem);

        //            // Decrease stock quantity (critical section - protected by transaction)
        //            product.StockQuantity -= itemDto.Quantity;
        //            await _productRepository.UpdateAsync(product);

        //            totalAmount += orderItem.TotalPrice;
        //        }

        //        order.TotalAmount = totalAmount;
        //        await _orderRepository.UpdateAsync(order);

        //        // Save all changes
        //        await _dbContext.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        // Fetch the complete order with related data
        //        var createdOrder = await _orderRepository
        //            .FindAndIncludeAsync(
        //                o => o.Id == order.Id,
        //                "OrderItems.Product",
        //                "Customer"
        //            );

        //        if (createdOrder is null || !createdOrder.Any())
        //        {
        //            response.StatusCode = StatusCodes.Status500InternalServerError;
        //            response.Message = "Failed to retrieve the created order.";
        //            return response;
        //        }

        //        var orderResponse = MapToOrderResponse(createdOrder.First());

        //        response.StatusCode = StatusCodes.Status201Created;
        //        response.Data = orderResponse;
        //        response.Message = Constants.SuccessMessage;

        //        _logger.LogInformation(
        //            "Order created successfully. OrderId: {OrderId}, OrderNumber: {OrderNumber}",
        //            order.Id,
        //            order.OrderNumber
        //        );

        //        return response;
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Concurrency conflict while creating order for customer {CustomerId}", request.CustomerId);

        //        response.StatusCode = StatusCodes.Status409Conflict;
        //        response.Message = Constants.ConcurentUpdate;
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);

        //        response.StatusCode = StatusCodes.Status500InternalServerError;
        //        response.Message = Constants.DefaultExceptionFriendlyMessage;
        //        return response;
        //    }
        //}


        public async Task<Response<OrderResponse>> CreateOrderAsync(CreateOrderDto request)
        {
            var response = new Response<OrderResponse>();

            var customer = await _userManager.FindByIdAsync(request.CustomerId);
            if (customer is null)
            {
                _logger.LogWarning("Customer not found with ID {UserId}", request.CustomerId);
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = Constants.CustomerNotFound;
                return response;
            }

            // Use the execution strategy to handle retries
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var productIds = request.OrderItems.Select(x => x.ProductId).ToList();

                    var products = await _productRepository
                        .FindByCondition(p => productIds.Contains(p.Id), trackChanges: true)
                        .ToListAsync();

                    if (products.Count != productIds.Distinct().Count())
                    {
                        response.StatusCode = StatusCodes.Status404NotFound;
                        response.Message = "One or more products not found.";
                        return response;
                    }
                    var insufficientStockItems = new List<string>();
                    var inactiveProducts = new List<string>();

                    foreach (var orderItem in request.OrderItems)
                    {
                        var product = products.FirstOrDefault(p => p.Id == orderItem.ProductId);

                        if (product == null)
                        {
                            insufficientStockItems.Add($"Product not found");
                            continue;
                        }

                        if (!product.IsActive)
                        {
                            inactiveProducts.Add($"{product.Name} is not available");
                        }

                        if (product.StockQuantity < orderItem.Quantity)
                        {
                            insufficientStockItems.Add(
                                $"{product.Name}: Requested {orderItem.Quantity}, Available {product.StockQuantity}"
                            );
                        }
                    }

                    if (inactiveProducts.Any())
                    {
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        response.Message = $"Inactive products: {string.Join(", ", inactiveProducts)}";
                        return response;
                    }

                    if (insufficientStockItems.Any())
                    {
                        response.StatusCode = StatusCodes.Status400BadRequest;
                        response.Message = $"Insufficient stock: {string.Join("; ", insufficientStockItems)}";
                        return response;
                    }

                    var order = new Orders
                    {
                        OrderNumber = GenerateOrderNumber(),
                        CustomerId = request.CustomerId,
                        Status = OrderStatus.Pending,
                        ShippingAddress = request.ShippingAddress,
                        Notes = request.Notes,
                        TotalAmount = 0
                    };

                    await _orderRepository.AddAsync(order);

                    decimal totalAmount = 0;
                    foreach (var itemDto in request.OrderItems)
                    {
                        var product = products.First(p => p.Id == itemDto.ProductId);

                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = product.Id,
                            Quantity = itemDto.Quantity,
                            UnitPrice = product.Price,
                            TotalPrice = product.Price * itemDto.Quantity
                        };

                        await _orderItemRepository.AddAsync(orderItem);

                        product.StockQuantity -= itemDto.Quantity;
                        await _productRepository.UpdateAsync(product);

                        totalAmount += orderItem.TotalPrice;
                    }

                    order.TotalAmount = totalAmount;
                    await _orderRepository.UpdateAsync(order);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Fetch the complete order
                    var createdOrder = await _orderRepository
                        .FindAndIncludeAsync(
                            o => o.Id == order.Id,
                            "OrderItems.Product",
                            "Customer"
                        );

                    var orderResponse = MapToOrderResponse(createdOrder.First());

                    response.StatusCode = StatusCodes.Status201Created;
                    response.Data = orderResponse;
                    response.Message = Constants.SuccessMessage;

                    _logger.LogInformation(
                        "Order created successfully. OrderId: {OrderId}, OrderNumber: {OrderNumber}",
                        order.Id,
                        order.OrderNumber
                    );

                    return response;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Concurrency conflict while creating order for customer {CustomerId}", request.CustomerId);

                    response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = Constants.ConcurentUpdate;
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);

                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = Constants.DefaultExceptionFriendlyMessage;
                    return response;
                }
            });
        }

        public async Task<Response<OrderResponse>> GetOrderByIdAsync(Guid id, string userId)
        {
            var response = new Response<OrderResponse>();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                {
                    _logger.LogWarning("User not found with ID {UserId}", userId);
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = Constants.CustomerNotFound;
                    return response;
                }

                var orders = await _orderRepository.FindAndIncludeAsync(
                    o => o.Id == id,
                    "OrderItems.Product",
                    "Customer"
                );

                var order = orders.FirstOrDefault();

                if (order is null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Order not found.";
                    return response;
                }

                if (user.UserType is not UserType.Admin && order.CustomerId != userId)
                {
                    _logger.LogWarning(
                       "User {UserId} attempted to access order {OrderId} belonging to {CustomerId}",
                       userId, id, order.CustomerId
                   );
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You do not have permission to view this order.";
                    return response;
                }
                    
                var orderResponse = MapToOrderResponse(order);

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = orderResponse;
                response.Message = Constants.SuccessMessage;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID: {OrderId}", id);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }


        public async Task<Response<PagedResult<OrderResponse>>> GetAllOrdersAsync(OrderFilterDto filter)
        {
            var response = new Response<PagedResult<OrderResponse>>();

            try
            {
                var pagedResult = await _orderRepository.GetPagedAsync(
                    filter.Page,
                    filter.PageSize,
                    filter: o =>
                        (string.IsNullOrEmpty(filter.CustomerId) || o.CustomerId == filter.CustomerId) &&
                        (!filter.Status.HasValue || o.Status == filter.Status.Value) &&
                        (!filter.FromDate.HasValue || o.DateCreated >= filter.FromDate.Value) &&
                        (!filter.ToDate.HasValue || o.DateCreated <= filter.ToDate.Value),
                    orderBy: q => q.OrderByDescending(o => o.DateCreated),
                    includeProperties: new[] { "OrderItems.Product", "Customer" }
                );

                var mappedResult = new PagedResult<OrderResponse>
                {
                    CurrentPage = pagedResult.CurrentPage,
                    PageSize = pagedResult.PageSize,
                    TotalCount = pagedResult.TotalCount,
                    TotalPages = pagedResult.TotalPages,
                    Items = pagedResult.Items.Select(MapToOrderResponse).ToList()
                };

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = mappedResult;
                response.Message = Constants.SuccessMessage;

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        public async Task<Response<PagedResult<OrderResponse>>> GetCustomerOrdersAsync(OrderFilterDto filter)
        {
            filter.CustomerId = filter.CustomerId;
            return await GetAllOrdersAsync(filter);
        }

        public async Task<Response<bool>> CancelOrderAsync(Guid id, string userId)
        {
            var response = new Response<bool>();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                {
                    _logger.LogWarning("User not found with ID {UserId}", user);
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = Constants.CustomerNotFound;
                    return response;
                }

                var orders = await _orderRepository.FindAndIncludeAsync(
                    o => o.Id == id,
                    "OrderItems.Product"
                );

                var order = orders.FirstOrDefault();

                if (order == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Order not found.";
                    return response;
                }

                if (user.UserType is not UserType.Admin && order.CustomerId != userId)
                {
                    _logger.LogWarning(
                         "User {UserId} attempted to cancel order {OrderId} belonging to {CustomerId}",
                         userId, id, order.CustomerId
                     );
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    response.Message = "You do not have permission to cancel this order.";
                    return response;
                }

                if (order.Status == OrderStatus.Cancelled)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Order is already cancelled.";
                    return response;
                }

                if (order.Status == OrderStatus.Completed)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Cannot cancel a completed order.";
                    return response;
                }

                // Restore stock quantities
                foreach (var orderItem in order.OrderItems)
                {
                    var product = orderItem.Product;
                    product.StockQuantity += orderItem.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                // Update order status
                order.Status = OrderStatus.Cancelled;
                order.CancelledDate = DateTime.UtcNow;
                await _orderRepository.UpdateAsync(order);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = true;
                response.Message = "Order cancelled successfully.";

                _logger.LogInformation("Order cancelled successfully. OrderId: {OrderId}", id);

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling order with ID: {OrderId}", id);

                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        private OrderResponse MapToOrderResponse(Orders order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                CustomerName = $"{order.Customer?.FirstName} {order.Customer?.LastName}".Trim(),
                CustomerEmail = order.Customer?.Email ?? string.Empty,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Notes = order.Notes,
                DateCreated = order.DateCreated,
                CompletedDate = order.CompletedDate,
                CancelledDate = order.CancelledDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice
                }).ToList()
            };
        }
    }
}
