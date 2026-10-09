using System.Text.Json;
using RequestAuditApp.Data;
using RequestAuditApp.Middleware;
using RequestAuditApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<InMemoryAuditDatabase>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IRequestFilter, AdminRequestFilter>();

var app = builder.Build();

app.UseExceptionHandler(branch => branch.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { message = "Внутренняя ошибка сервера." });
}));

app.UseMiddleware<RequestAuditMiddleware>();

app.MapGet("/", () => Results.Ok(new
{
    application = "Аудит HTTP-запросов",
    storage = "Имитация БД в памяти. При остановке приложения записи удаляются.",
    logging = "Все входящие запросы",
    auditRules = new[] { "Префикс /admin", "Заголовок X-Audit: true" },
    timestamp = "UTC",
    paths = new[] { "/admin", "/admin/status", "/admin/error", "/audit", "/audit/summary", "/api/echo" }
}));

app.MapGet("/admin", () => Results.Ok(new { message = "Панель администратора" }));

app.MapGet("/admin/status", () => Results.Ok(new
{
    status = "Работает",
    serverTime = DateTime.UtcNow
}));

app.MapGet("/admin/error", (HttpContext context) =>
    Task.FromException(new InvalidOperationException("Тестовая ошибка для проверки аудита.")));

app.MapPost("/api/echo", (JsonElement data) => Results.Ok(new { received = data }));

app.MapGet("/audit", async (IAuditService auditService) =>
    Results.Ok(await auditService.GetRecordsAsync()));

app.MapGet("/audit/summary", async (IAuditService auditService) =>
{
    var records = await auditService.GetRecordsAsync();
    return Results.Ok(new
    {
        total = records.Count,
        errors = records.Count(record => record.StatusCode >= 400),
        averageDurationMs = records.Count == 0 ? 0 : Math.Round(records.Average(record => record.DurationMs), 2),
        methods = records.GroupBy(record => record.Method)
            .ToDictionary(group => group.Key, group => group.Count())
    });
});

app.Run();
