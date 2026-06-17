using System.Text.Json;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Exceptions;

namespace Referral_Management.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var statusCode = 500;

        if (exception is ApiException apiException)
        {
            statusCode = apiException.StatusCode;
        }

        var response = new ApiErrorResponseDTO
            {
                Success = false,
                Message = exception.Message,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow
            };

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}