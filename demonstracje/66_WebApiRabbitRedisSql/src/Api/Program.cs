using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QueueCacheDemo;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql"), sql => sql.EnableRetryOnFailure()));
builder.Services.AddStackExchangeRedisCache(o => o.Configuration = builder.Configuration.GetConnectionString("Redis")!);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new {status = "running"}));

app.MapPost("/products", async (CreateProductRequest request, IConfiguration config, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200 || request.Price < 0 || request.Price > 9999999999999999m)
        return Results.BadRequest(new { error = "Name (1..200) and nonnegative price are required." });

    var operationId = Guid.NewGuid();
    var productId = Guid.NewGuid();
    var command = new CreateProduct(operationId, productId, request.Name.Trim(), request.Price);
    var factory = new ConnectionFactory
    {
        HostName = config["Rabbit:Host"] ?? "localhost",
        UserName = "demo",
        Password = "demo"
    };
    try
    {
        await using var connection = await factory.CreateConnectionAsync(ct);
        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true), ct);
        await channel.QueueDeclareAsync("products.create", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { ["x-dead-letter-exchange"] = "products.dlx" }, cancellationToken: ct);
        var props = new BasicProperties { Persistent = true, MessageId = operationId.ToString() };
        await channel.BasicPublishAsync("", "products.create", true, props,
            JsonSerializer.SerializeToUtf8Bytes(command), ct);
        return Results.Accepted($"/operations/{operationId}", new { operationId, productId, status = "accepted" });
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        app.Logger.LogError(ex, "RabbitMQ publish failed");
        return Results.Problem("Message could not be confirmed by broker.", statusCode: 503);
    }
});

app.MapGet("/products/{id:guid}", async (Guid id, AppDbContext db, IDistributedCache cache, HttpContext http, CancellationToken ct) =>
{
    var key = $"products:{id:N}";
    try
    {
        var cached = await cache.GetStringAsync(key, ct);
        if (cached is not null)
        {
            http.Response.Headers["X-Cache"] = "HIT";
            return Results.Content(cached, "application/json");
        }
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        app.Logger.LogWarning(ex, "Redis unavailable, reading SQL directly");
    }

    // No negative caching: a queued command might not be committed yet.
    var p = await db.Products.AsNoTracking().Where(p => p.Id == id)
        .Select(p => new ProductDto(p.Id, p.Name, p.Price)).SingleOrDefaultAsync(ct);
    http.Response.Headers["X-Cache"] = "MISS";
    if (p is null) return Results.NotFound();

    var json = JsonSerializer.Serialize(p, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    try
    {
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15) }, ct);
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        app.Logger.LogWarning(ex, "Redis SET failed");
    }
    return Results.Content(json, "application/json");
});

app.MapGet("/operations/{id:guid}", async (Guid id, AppDbContext db, CancellationToken ct) =>
{
    var completed = await db.ProcessedCommands.AsNoTracking().AnyAsync(x => x.Id == id, ct);
    return Results.Ok(new { operationId = id, status = completed ? "completed" : "pending_or_unknown" });
});

app.Run();
public sealed record CreateProductRequest(string Name, decimal Price);
