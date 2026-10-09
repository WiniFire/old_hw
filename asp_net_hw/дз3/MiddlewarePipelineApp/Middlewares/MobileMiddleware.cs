namespace MiddlewarePipelineApp.Middlewares;

public class MobileMiddleware
{
    private readonly RequestDelegate _next;

    public MobileMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public static bool IsMobile(HttpRequest request)
    {
        var userAgent = request.Headers.UserAgent.ToString();
        return userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase)
            || userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase)
            || userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
            || userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsMobile(context.Request))
        {
            await _next(context);
            return;
        }

        context.Response.Headers["X-Pipeline-Branch"] = "Mobile";
        await context.Response.WriteAsJsonAsync(new
        {
            handler = nameof(MobileMiddleware),
            message = "Мобильная версия ответа",
            device = "mobile",
            layout = "compact",
            path = context.Request.Path.ToString(),
            userAgent = context.Request.Headers.UserAgent.ToString(),
            serverTime = DateTime.Now
        });
    }
}
