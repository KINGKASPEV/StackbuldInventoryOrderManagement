using Microsoft.AspNetCore.Http;

namespace StackbuldInventoryOrderManagement.Application.Middlewares
{
    public static class HttpContextExtensions
    {
        public static string GetClientIPAddress(this HttpContext context)
        {
            if (context == null)
                return string.Empty;

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (
                string.IsNullOrEmpty(ipAddress)
                && context.Request.Headers.ContainsKey("X-Forwarded-For")
            )
            {
                ipAddress = context
                    .Request.Headers["X-Forwarded-For"]
                    .ToString()
                    .Split(',')
                    .FirstOrDefault();
            }

            return ipAddress ?? string.Empty;
        }
    }
}
