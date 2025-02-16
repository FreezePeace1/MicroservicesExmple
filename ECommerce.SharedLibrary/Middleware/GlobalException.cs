using System.Net;
using System.Text.Json;
using ECommerceSharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceSharedLibrary.Middleware;

public class GlobalException
{
    private readonly RequestDelegate _next;

    public GlobalException(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Объявляем дефолтные переменные
        string message = "sorry, internal server error occured, " +
                         "please try again";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Error";

        try
        {
            await _next(context);
            
            // проверяем если в ответ поступило много запросов //429 status code
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "Too many requests made";
                statusCode = StatusCodes.Status429TooManyRequests;

                await ModifyHeader(context,title,message,statusCode);
            }
            
            // если в ответ поступило что пользователь не авторизован //401 status code
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "alert";
                message = "You are not authorized to access";
                statusCode = StatusCodes.Status401Unauthorized;

                await ModifyHeader(context, title, message, statusCode);
            }
            
            // если в ответ поступило Forbidden // 403 status code
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Access";
                message = "You are not allowed/required to access";
                statusCode = StatusCodes.Status403Forbidden;

                await ModifyHeader(context, title, message, statusCode);
            }
            
        }
        catch (Exception e)
        {
            //Log original exceptions / File, Debugger, Console
            LogException.LogExceptions(e);
            
            // check if exception is timeout типо долгое время ожидания // 408 request timeout
            if (e is TaskCanceledException || e is TimeoutException)
            {
                title = "Out of time";
                message = "Request timeout... try again";
                statusCode = StatusCodes.Status408RequestTimeout;
            }

            // if none of the exceptions then do the default самое первео это internal server error 500
            await ModifyHeader(context, title, message, statusCode);
        }
        
    }

    private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        // отображаем сообщение без угроз для пользователя?
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Detail = message,
            Status = statusCode,
            Title = title
        }),cancellationToken: CancellationToken.None);
        
    }
}