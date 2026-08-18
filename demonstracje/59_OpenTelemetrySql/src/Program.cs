using System.Diagnostics;
using Demo59;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);
builder.Services.AddSingleton(new ActivitySource("Demo59"));

if (!testing)
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<EventsDb>(o => o.UseSqlServer(cs));
    var otlp = builder.Configuration["Otlp:Endpoint"] ?? "http://localhost:4317";
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService("demo59-api"))
        .WithTracing(t => t
            .AddSource("Demo59")
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(o => o.Endpoint = new Uri(otlp)));
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventsDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/v1/events", async (CreateEvent dto, EventsDb db, ActivitySource source) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 3)
        return Results.BadRequest(new { error = "Name must have at least 3 characters." });
    using var activity = source.StartActivity("create-event");
    var row = new TraceEvent
    {
        Name = dto.Name.Trim(),
        TraceId = activity?.TraceId.ToString() ?? "none"
    };
    db.Events.Add(row);
    await db.SaveChangesAsync();
    activity?.SetTag("event.id", row.Id);
    return Results.Created($"/api/v1/events/{row.Id}", row);
});

app.MapGet("/api/v1/events", async (EventsDb db) =>
    await db.Events.AsNoTracking().OrderByDescending(e => e.Id).ToListAsync());

app.Run();

public sealed record CreateEvent(string Name);
public partial class Program;
