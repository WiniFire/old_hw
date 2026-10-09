using System.Diagnostics;

namespace MiddlewarePipelineApp.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var timer = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path.ToString();
        var failed = false;

        context.Response.Headers["X-Request-Logged"] = "true";
        _logger.LogInformation("Запрос: {Method} {Path}", method, path);

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            failed = true;
            _logger.LogError(exception, "Ошибка запроса: {Method} {Path}", method, path);
            throw;
        }
        finally
        {
            timer.Stop();
            _logger.LogInformation("Ответ: {Method} {Path}, статус {StatusCode}, время {ElapsedMs} мс",
                method, path, failed ? 500 : context.Response.StatusCode, timer.ElapsedMilliseconds);
        }
    }
}
