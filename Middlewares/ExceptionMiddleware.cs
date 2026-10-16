using System.Net;
using System.Text.Json;

namespace AuthDemo.Api.Middlewares;

/// <summary>
/// Middleware xử lý Exception toàn cục (Global Exception Handler).
/// Bắt mọi exception chưa được xử lý trong pipeline và trả về JSON chuẩn.
/// 
/// Cấu hình trong Program.cs (đặt TRƯỚC app.UseAuthentication()):
///     app.UseMiddleware<ExceptionMiddleware>();
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Chuyển request sang middleware tiếp theo trong pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Ghi log lỗi chi tiết phía server
            _logger.LogError(ex, "Đã xảy ra lỗi không mong muốn: {Message}", ex.Message);

            // Trả về response lỗi chuẩn cho client
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Xác định status code dựa trên loại exception
        var statusCode = exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,       // 401
            KeyNotFoundException       => (int)HttpStatusCode.NotFound,            // 404
            ArgumentException          => (int)HttpStatusCode.BadRequest,          // 400
            _                          => (int)HttpStatusCode.InternalServerError  // 500
        };

        // Tạo response body JSON chuẩn
        var response = new
        {
            success = false,
            message = exception.Message,
            data = (object?)null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }
}
