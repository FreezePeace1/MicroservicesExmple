using Microsoft.AspNetCore.Http;

namespace ECommerceSharedLibrary.Middleware;

public class ListenToOnlyApiGateway
{
    private readonly RequestDelegate _next;

    public ListenToOnlyApiGateway(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Извлекаем определенный заголовок из запроса
        // extract specific header from the request
        var signedHeader = context.Request.Headers["Api-Gateway"];
        
        // null means, the request is not coming from the Api Gateway
        if (signedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

            await context.Response.WriteAsync("Sorry service is unavailable");
        }
        else
        {
            await _next(context);
        }

    }
}