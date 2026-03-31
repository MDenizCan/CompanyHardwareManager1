using System.Net;
using System.Text.Json;
using Serilog;

namespace CHM.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
        _logger = Log.ForContext<GlobalExceptionMiddleware>();
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // İsteği bir sonraki adıma ilet
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // Eğer herhangi bir adımda hata olursa buraya düşer ve Serilog ile loglanır
            _logger.Error(ex, "API İsteği sırasında beklenmeyen bir hata oluştu: {Message}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Hata tipine göre uygun HTTP statü kodunu belirle
        context.Response.StatusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            InvalidOperationException or ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "İşlem sırasında bir hata meydana geldi.",
            Details = exception.Message // Geliştirme kolaylığı için hata mesajını geri dönüyoruz.
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
