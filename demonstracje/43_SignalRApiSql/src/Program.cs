using Demo43;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);

builder.Services.AddSignalR();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true)));

if (!testing)
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<ChatDb>(o => o.UseSqlServer(cs));
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

app.UseCors();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/v1/messages", async (ChatDb db) =>
    await db.Messages.AsNoTracking().OrderBy(m => m.Id).ToListAsync());

app.MapHub<ChatHub>("/hubs/chat");
app.Run();

public partial class Program;
