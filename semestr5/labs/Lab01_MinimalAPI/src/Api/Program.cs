var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () =>
    Results.Ok(new { status = "ok", utc = DateTime.UtcNow }));

app.MapGet("/hello", (string? name) =>
{
    var who = string.IsNullOrWhiteSpace(name) ? "world" : name.Trim();
    return Results.Ok(new { message = $"Hello, {who}" });
});

app.Run();

public partial class Program;
