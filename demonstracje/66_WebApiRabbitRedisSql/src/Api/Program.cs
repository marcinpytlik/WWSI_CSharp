using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QueueCacheDemo.Api.Middleware;
using QueueCacheDemo.Contracts;
using QueueCacheDemo.Data;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sql"),
        sql => sql.EnableRetryOnFailure()));
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Logging wraps exception handling, so errors are logged with final HTTP 500 status.
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapGet("/health", () => Results.Ok(new { status = "running" }));

app.MapPost("/products", async (CreateProductRequest request, IConfiguration configuration, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200 ||
        request.Price < 0 || request.Price > 9999999999999999m)
    {
        return Results.BadRequest(new { error = "Name (1..200) and price (0..9999999999999999) are required." });
    }

    var operationId = Guid.NewGuid();
    var productId = Guid.NewGuid();
    var command = new CreateProduct(operationId, productId, request.Name.Trim(), request.Price);
    var factory = new ConnectionFactory
    {
        HostName = configuration["Rabbit:Host"] ?? "localhost",
        UserName = configuration["Rabbit:User"] ?? throw new InvalidOperationException("Rabbit:User not configured"),
        Password = configuration["Rabbit:Password"] ?? throw new InvalidOperationException("Rabbit:Password not configured")
    };

    try
    {
        await using var connection = await factory.CreateConnectionAsync(ct);
        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true), ct);
        await channel.QueueDeclareAsync("products.create", durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { ["x-dead-letter-exchange"] = "products.dlx" },
            cancellationToken: ct);
        await channel.BasicPublishAsync("",
            routingKey: "products.create", mandatory: true,
            basicProperties: new BasicProperties { Persistent = true, MessageId = operationId.ToString() },
            body: JsonSerializer.SerializeToUtf8Bytes(command), cancellationToken: ct);

        return Results.Accepted($"/operations/{operationId}",
            new { operationId, productId, status = "accepted" });
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        app.Logger.LogError(ex, "RabbitMQ publication failed for {OperationId}", operationId);
        return Results.Problem(
            title: "Messaging unavailable",
            detail: "The broker did not confirm acceptance of the command.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.MapGet("/products/{id:guid}",
    async (Guid id, AppDbContext db, IDistributedCache cache, HttpContext http, CancellationToken ct) =>
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
            app.Logger.LogWarning(ex, "Redis cache unavailable; read will use SQL Server");
        }

        var product = await db.Products.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProductDto(x.Id, x.Name, x.Price))
            .SingleOrDefaultAsync(ct);

        http.Response.Headers["X-Cache"] = "MISS";
        if (product is null)
        {
            // Do not cache 404 while an accepted command could still be in the queue.
            return Results.NotFound();
        }

        var json = JsonSerializer.Serialize(product, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        try
        {
            await cache.SetStringAsync(key, json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
                }, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            app.Logger.LogWarning(ex, "Redis cache write failed");
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
