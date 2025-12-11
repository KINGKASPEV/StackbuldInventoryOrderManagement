using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StackbuldInventoryOrderManagement.Application.Interfaces.Repositories;

namespace StackbuldInventoryOrderManagement.Application.Middlewares
{
    public class LogUtil(RequestDelegate next, IAuditTrailRepository auditTrail)
    {
        public Task Invoke(HttpContext httpContext)
        {
            var httpVerb = httpContext.Request.Method;
            var method = httpContext.Request.Path;
            var user = httpContext.User.Identity!.Name ?? "anonymous";
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var controllerName = httpContext.GetRouteData().Values["controller"]?.ToString();
            var actionName = httpContext.GetRouteData().Values["action"]?.ToString();

            auditTrail.LogAction(
                controllerName!,
                $"{httpVerb} call on controller method {controllerName}.{actionName}",
                method.ToString(),
                user,
                ipAddress!
            );
            return next(httpContext);
        }
    }

}
