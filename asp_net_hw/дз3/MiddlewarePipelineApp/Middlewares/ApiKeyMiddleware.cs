namespace MiddlewarePipelineApp.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["ApiSecurity:ApiKey"]
            ?? throw new InvalidOperationException("Не настроен API-ключ.");
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("API-ключ не может быть пустым.");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var suppliedKey = context.Request.Headers["X-Api-Key"];

        if (suppliedKey.Count != 1 || !string.Equals(suppliedKey[0], _apiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                handler = nameof(ApiKeyMiddleware),
                message = "Отсутствует или неверен заголовок X-Api-Key."
            });
            return;
        }

        context.Response.Headers["X-Api-Key-Validated"] = "true";
        await _next(context);
    }
}
