using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.Services;
using StackbuldInventoryOrderManagement.Application.User.Validator;
using System.Reflection;

namespace StackbuldInventoryOrderManagement.Application.Middlewares
{
    public static class RegisterFluentValidationServices
    {
        public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddValidatorsFromAssemblyContaining<IAuthenticationService>();


            return services;
        }
    }
}
