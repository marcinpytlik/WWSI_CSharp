using Demo60;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

namespace Demo60.Grpc;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var testing = builder.Configuration.GetValue("Testing", false);

        builder.WebHost.ConfigureKestrel(k =>
            k.ConfigureEndpointDefaults(e => e.Protocols = HttpProtocols.Http1AndHttp2));
        builder.Services.AddGrpc();
        builder.Services.AddScoped<NotesApp>();

        if (!testing)
        {
            var cs = builder.Configuration.GetConnectionString("App")
                     ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
            builder.Services.AddDbContext<NotesDb>(o => o.UseSqlServer(cs));
        }

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NotesDb>();
            await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
        }

        app.MapGet("/health", () => Results.Ok(new { status = "ok", role = "grpc" }));
        app.MapGrpcService<NotesGrpcService>();
        await app.RunAsync();
    }
}
