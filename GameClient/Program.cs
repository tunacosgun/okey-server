// GameServer/Program.cs
using GameServer.Hubs;
using GameServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = 64 * 1024;
});
builder.Services.AddCors(o => o.AddPolicy("dev",
    p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddSingleton<TableService>();

var app = builder.Build();

app.UseCors("dev");
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok("OK"));
app.MapHub<GameHub>("/hub");

app.Run();