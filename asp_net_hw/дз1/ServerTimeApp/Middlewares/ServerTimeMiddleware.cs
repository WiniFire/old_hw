using System.Globalization;

namespace ServerTimeApp.Middlewares;

public class ServerTimeMiddleware
{
    private readonly RequestDelegate _next;

    public ServerTimeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/")
        {
            var currentTime = DateTime.Now;
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.Headers.CacheControl = "no-store";

            await context.Response.WriteAsync(
                "Дата и время сервера: " +
                currentTime.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture));

            return;
        }

        await _next(context);
    }
}
