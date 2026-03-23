using System;
using System.Net.Mime;
using System.Text.Json;

namespace CarStock.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Unhandled error has occured");
            await HandleExceptionAsync(context, ex);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var message = _env.IsDevelopment() ? ex.Message : "An error Occured. Please contact the developer";

        var result = JsonSerializer.Serialize(new
        {
            StatusCode = context.Response.StatusCode,
            Message = message,
            Detail = _env.IsDevelopment() ? ex.StackTrace : null,
        });
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
}
