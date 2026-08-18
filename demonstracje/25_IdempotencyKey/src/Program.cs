using System.Collections.Concurrent;

var orders = new ConcurrentDictionary<Guid, Order>();
var idempotency = new ConcurrentDictionary<string, Guid>(StringComparer.Ordinal);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/v1/orders", (HttpRequest request, CreateOrder dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Sku) || dto.Qty <= 0)
        return Results.BadRequest(new { error = "Sku and positive Qty required." });

    var key = request.Headers["Idempotency-Key"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(key))
        return Results.BadRequest(new { error = "Idempotency-Key header is required." });

    if (idempotency.TryGetValue(key, out var existingId) && orders.TryGetValue(existingId, out var existing))
        return Results.Ok(existing);

    var order = new Order(Guid.NewGuid(), dto.Sku.Trim(), dto.Qty);
    if (!idempotency.TryAdd(key, order.Id))
        return Results.Ok(orders[idempotency[key]]);

    orders[order.Id] = order;
    return Results.Created($"/api/v1/orders/{order.Id}", order);
});

app.Run();

public sealed record CreateOrder(string Sku, int Qty);
public sealed record Order(Guid Id, string Sku, int Qty);
public partial class Program;
