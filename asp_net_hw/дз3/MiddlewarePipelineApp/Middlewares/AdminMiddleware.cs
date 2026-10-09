namespace MiddlewarePipelineApp.Middlewares;

public class AdminMiddleware
{
    private readonly RequestDelegate _next;

    public AdminMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["X-Pipeline-Branch"] = "Admin";

        if (context.Request.Path == "" || context.Request.Path == "/")
        {
            await context.Response.WriteAsJsonAsync(new
            {
                handler = nameof(AdminMiddleware),
                message = "Панель администратора",
                pathBase = context.Request.PathBase.ToString(),
                availablePaths = new[] { "/admin", "/admin/status" }
            });
            return;
        }

        if (context.Request.Path == "/status")
        {
            await context.Response.WriteAsJsonAsync(new
            {
                handler = nameof(AdminMiddleware),
                application = "MiddlewarePipelineApp",
                status = "Работает",
                serverTime = DateTime.Now,
                pathBase = context.Request.PathBase.ToString(),
                path = context.Request.Path.ToString()
            });
            return;
        }

        await _next(context);
    }
}
