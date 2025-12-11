using Microsoft.Extensions.DependencyInjection;
using StackbuldInventoryOrderManagement.Application.Interfaces.Persistence;
using StackbuldInventoryOrderManagement.Application.Interfaces.Repositories;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.Services;
using StackbuldInventoryOrderManagement.Persistence.Context;

namespace StackbuldInventoryOrderManagement.Persistence.Repositories
{
    public static class Extensions
    {
        public static void AddRepository(this IServiceCollection services)
        {

            // Dapper Repository Registration
            services.AddTransient<IAuditTrailRepository, AuditTrailRepository>();
            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }

}
