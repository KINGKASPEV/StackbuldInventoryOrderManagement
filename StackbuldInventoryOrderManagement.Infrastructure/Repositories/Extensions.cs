using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using StackbuldInventoryOrderManagement.Application.Interfaces.Persistence;
using StackbuldInventoryOrderManagement.Application.Interfaces.Repositories;
using StackbuldInventoryOrderManagement.Persistence.Context;

namespace StackbuldInventoryOrderManagement.Persistence.Repositories
{
    public static class Extensions
    {
        public static void AddRepository(this IServiceCollection services)
        {

            // Dapper Repository Registration
            services.AddTransient<IAuditTrailRepository, AuditTrailRepository>();

            // DbContext Registration
            services.AddScoped<IAppDbContext, AppDbContext>();

            // Repository Registration
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
           // services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddHttpContextAccessor();

            // Services Middleware Registration
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }

}
