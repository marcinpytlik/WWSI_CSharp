using Demo11;
using Demo11.Consumer;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Orders")));
builder.Services.AddScoped<IOrderStore, SqlOrderStore>();
builder.Services.AddScoped<OrderProcessor>();
builder.Services.AddHostedService<OrderConsumerHostedService>();

var host = builder.Build();
var startupLogger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

await using (var scope = host.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await RetryAsync(
        () => db.Database.EnsureCreatedAsync(),
        startupLogger,
        "SQL Server (EnsureCreated)",
        CancellationToken.None);
}

await host.RunAsync();

static async Task RetryAsync(
    Func<Task> action,
    ILogger logger,
    string what,
    CancellationToken cancellationToken)
{
    for (var attempt = 1; ; attempt++)
    {
        try
        {
            await action();
            return;
        }
        catch (Exception ex) when (attempt < 20 && !cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "{What} niegotowe (próba {Attempt}/20). Ponawiam za 3 s…", what, attempt);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        }
    }
}
