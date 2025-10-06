namespace UserManagementAPI.Middleware;

public class RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        logger.LogInformation("Request: {method} {path}", context.Request.Method, context.Request.Path);

        await next(context);

        logger.LogInformation("Response Status: {statusCode}", context.Response.StatusCode);
    }
}

