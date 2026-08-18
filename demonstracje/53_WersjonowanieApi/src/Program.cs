var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/v1/hello", () => Results.Ok(new { version = 1, message = "hello" }));
app.MapGet("/api/v2/hello", () => Results.Ok(new { version = 2, message = "cześć", lang = "pl" }));

app.Run();

public partial class Program;
