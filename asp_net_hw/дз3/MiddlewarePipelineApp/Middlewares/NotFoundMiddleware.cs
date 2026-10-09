namespace MiddlewarePipelineApp.Middlewares;

public class NotFoundMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new
        {
            handler = nameof(NotFoundMiddleware),
            message = "Страница не найдена.",
            path = context.Request.PathBase.ToString() + context.Request.Path.ToString()
        });
    }
}
