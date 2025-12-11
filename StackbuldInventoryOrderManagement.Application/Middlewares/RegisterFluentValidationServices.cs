using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StackbuldInventoryOrderManagement.Application.User.Validator;
using System.Reflection;

namespace StackbuldInventoryOrderManagement.Application.Middlewares
{
    public static class RegisterFluentValidationServices
    {
        public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            //services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


            return services;
        }
    }
}
