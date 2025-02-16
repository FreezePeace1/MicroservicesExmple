namespace Gateway.Api.Middleware;

public class AttachSignatureToRequest
{
    private readonly RequestDelegate _next;

    public AttachSignatureToRequest(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers["Api-Gateway"] = "Signed";

        await _next(context);
    }
}