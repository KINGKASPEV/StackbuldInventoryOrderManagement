using Microsoft.AspNetCore.Builder;

namespace StackbuldInventoryOrderManagement.Application.Middlewares
{
    public static class LogUtilExtensions
    {
        public static IApplicationBuilder UseLogUtil(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogUtil>();
        }
    }

}
