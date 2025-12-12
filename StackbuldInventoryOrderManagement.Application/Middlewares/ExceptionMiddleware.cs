using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackbuldInventoryOrderManagement.Common.Responses;
using System.Net;
using Newtonsoft.Json;
using ApplicationException = StackbuldInventoryOrderManagement.Common.CustomException.ApplicationException;

namespace StackbuldInventoryOrderManagement.Application.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (ApplicationException ex)
            {
                logger.LogError(ex, "Something went wrong");
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Something went wrong");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            ApplicationException exception
        )
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)exception.StatusCode;
            var message = exception.Message;
            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(
            new Response<object> { StatusCode = (int)exception.StatusCode, Message = message }
                )
            );
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = exception switch
            {
                BadHttpRequestException => "Invalid request payload supplied",
                NotImplementedException => "Method not implemented in logic",
                ApplicationException => exception.Message,
                UnauthorizedAccessException =>
                    "User does not have required permission to access this endpoint",
                _ => exception.Message,
            };

            await context.Response.WriteAsync(
                JsonConvert.SerializeObject(
                    new Response<object>
                    {
                        StatusCode = Enum.Parse<int>(context.Response.StatusCode.ToString()),
                        Message = message,
                    }
                )
            );
        }
    }
}
