using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middleware;

public class TokenAuthenticationMiddleware(ILogger<TokenAuthenticationMiddleware> logger) : IMiddleware
{
    private const string TokenHeader = "Authorization";
    private const string ValidToken = "Bearer your-secret-token";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(TokenHeader, out var token) || token != ValidToken)
        {
            logger.LogWarning("Unauthorized access attempt.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await next(context);
    }
}
