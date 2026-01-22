using System.Text.Json;
using FluentValidation;
using TodoApp.Domain.Domain.Exceptions;

namespace TodoApp.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, apiResponse) = exception switch
        {
            ValidationException => (400, new { success = false, message = "Validation failed", errors = new List<string>() }),
            DomainException domainEx => (400, new { success = false, message = domainEx.Message, errors = new List<string>() }),
            ArgumentException argEx => (400, new { success = false, message = argEx.Message, errors = new List<string>() }),
            _ => (500, new { success = false, message = "Internal server error", errors = new List<string>() })
        };

        response.StatusCode = statusCode;
        await response.WriteAsync(JsonSerializer.Serialize(apiResponse));
    }
}
