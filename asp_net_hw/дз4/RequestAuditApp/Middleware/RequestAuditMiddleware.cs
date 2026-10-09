using System.Diagnostics;
using RequestAuditApp.Models;
using RequestAuditApp.Services;

namespace RequestAuditApp.Middleware;

public class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestAuditMiddleware> _logger;

    public RequestAuditMiddleware(RequestDelegate next, ILogger<RequestAuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService, IRequestFilter requestFilter)
    {
        var timestamp = DateTime.UtcNow;
        var path = context.Request.PathBase.ToString() + context.Request.Path.ToString();
        var method = context.Request.Method;
        var user = context.User.Identity?.IsAuthenticated == true
            ? context.User.Identity.Name ?? "Anonymous"
            : "Anonymous";
        var shouldAudit = requestFilter.ShouldAudit(context);
        var timer = Stopwatch.StartNew();
        var failed = false;

        _logger.LogInformation("Входящий запрос: {Method} {Path}, пользователь {User}", method, path, user);

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            failed = true;
            _logger.LogError(exception, "Ошибка запроса {Method} {Path}", method, path);
            throw;
        }
        finally
        {
            timer.Stop();
            var statusCode = failed ? StatusCodes.Status500InternalServerError : context.Response.StatusCode;

            _logger.LogInformation("Завершён запрос: {Method} {Path}, статус {StatusCode}, время {DurationMs} мс, сохранение {ShouldAudit}",
                method, path, statusCode, timer.ElapsedMilliseconds, shouldAudit);

            if (shouldAudit)
            {
                await auditService.SaveAsync(new AuditRecord
                {
                    Path = path,
                    Method = method,
                    User = user,
                    Timestamp = timestamp,
                    StatusCode = statusCode,
                    DurationMs = timer.ElapsedMilliseconds
                });
            }
        }
    }
}
