using ServerTimeApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<ServerTimeMiddleware>();

app.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    context.Response.ContentType = "text/plain; charset=utf-8";
    await context.Response.WriteAsync("Страница не найдена.");
});

app.Run();
