
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();

var items = new List<string>();
app.MapGet("/api/v1/health", () => Results.Ok(new { status = "ok" }));
app.MapPost("/api/v1/items", (string v) => { items.Add(v); return Results.Created($"/api/v1/items/{items.Count-1}", v); });
app.MapGet("/api/v1/items", () => Results.Ok(items));

app.Run();
public partial class Program {}

app.MapGet("/healthz/live", () => Results.Ok(new { status = "live" }));
app.MapGet("/healthz/ready", () => Results.Ok(new { status = "ready" }));
