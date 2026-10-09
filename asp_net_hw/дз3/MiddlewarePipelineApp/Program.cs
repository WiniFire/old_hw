using MiddlewarePipelineApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(next =>
{
    var logger = app.Services.GetRequiredService<ILogger<RequestLoggingMiddleware>>();
    var middleware = new RequestLoggingMiddleware(next, logger);
    return middleware.InvokeAsync;
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), branch =>
{
    branch.UseMiddleware<ApiKeyMiddleware>();
});

app.Map("/admin", branch =>
{
    branch.UseMiddleware<AdminMiddleware>();
    branch.Run(new NotFoundMiddleware().InvokeAsync);
});

app.MapWhen(context => MobileMiddleware.IsMobile(context.Request), branch =>
{
    branch.UseMiddleware<MobileMiddleware>();
});

app.UseMiddleware<MainMiddleware>();
app.Run(new NotFoundMiddleware().InvokeAsync);

app.Run();
