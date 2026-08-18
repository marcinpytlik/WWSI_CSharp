var hits = 0;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOutputCache();
var app = builder.Build();
app.UseOutputCache();

app.MapGet("/api/v1/ticks", () =>
{
    var n = Interlocked.Increment(ref hits);
    return Results.Ok(new { n });
}).CacheOutput(o => o.Expire(TimeSpan.FromSeconds(30)));

app.Run();

public partial class Program;
