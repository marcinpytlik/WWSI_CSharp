using Demo42;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);

builder.Host.UseSerilog((ctx, services, lc) =>
{
    lc.ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
    var seqUrl = ctx.Configuration["Seq:Url"];
    if (!testing && !string.IsNullOrWhiteSpace(seqUrl))
        lc.WriteTo.Seq(seqUrl);
});

if (!testing)
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<EventsDb>(o => o.UseSqlServer(cs));
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventsDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

app.Use(async (ctx, next) =>
{
    var cid = ctx.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
    ctx.Response.Headers["X-Correlation-Id"] = cid;
    using var logScope = app.Logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = cid });
    ctx.Items["cid"] = cid;
    await next();
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/v1/events", async (CreateEvent dto, EventsDb db, HttpContext ctx, ILoggerFactory logFactory) =>
{
    if (string.IsNullOrWhiteSpace(dto.Message) || dto.Message.Trim().Length < 3)
        return Results.BadRequest(new { error = "Message must have at least 3 characters." });
    var cid = ctx.Items["cid"] as string ?? "";
    var row = new EventRow { Message = dto.Message.Trim(), CorrelationId = cid };
    db.Events.Add(row);
    await db.SaveChangesAsync();
    logFactory.CreateLogger("Events").LogInformation("Zapisano zdarzenie {Id} {CorrelationId}", row.Id, cid);
    return Results.Created($"/api/v1/events/{row.Id}", row);
});

app.MapGet("/api/v1/events", async (EventsDb db) =>
    await db.Events.AsNoTracking().OrderByDescending(e => e.Id).ToListAsync());

app.Run();

public sealed record CreateEvent(string Message);
public partial class Program;
