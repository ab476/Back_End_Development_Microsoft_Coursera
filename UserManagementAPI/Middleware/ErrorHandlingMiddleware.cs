using System.Net;
using System.Text.Json;

namespace UserManagementAPI.Middleware;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = JsonSerializer.Serialize(new { error = "Internal server error." });
            await context.Response.WriteAsync(errorResponse);
        }
    }
}

