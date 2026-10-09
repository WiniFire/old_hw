namespace MiddlewarePipelineApp.Middlewares;

public class MainMiddleware
{
    private readonly RequestDelegate _next;

    public MainMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["X-Pipeline-Branch"] = "Main";

        if (context.Request.Path == "/")
        {
            await context.Response.WriteAsJsonAsync(new
            {
                handler = nameof(MainMiddleware),
                message = "Система обработки запросов",
                paths = new[] { "/", "/api/data", "/admin", "/admin/status" },
                apiHeader = "X-Api-Key",
                mobileDetection = "User-Agent: Mobile, Android, iPhone или iPad"
            });
            return;
        }

        if (context.Request.Path == "/api/data")
        {
            await context.Response.WriteAsJsonAsync(new
            {
                handler = nameof(MainMiddleware),
                message = "API-ключ проверен. UseWhen вернул запрос в основной конвейер.",
                data = new[] { "Товар 1", "Товар 2", "Товар 3" }
            });
            return;
        }

        await _next(context);
    }
}
