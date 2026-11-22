using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Lab")
    ?? throw new InvalidOperationException("Ustaw ConnectionStrings:Lab.");

builder.Services.AddSingleton<SqlCommandCounter>();
builder.Services.AddDbContext<LabDbContext>((sp, options) =>
    options.UseSqlServer(connectionString).AddInterceptors(sp.GetRequiredService<SqlCommandCounter>()));

var app = builder.Build();

// Demo izolowane w Dockerze: jawnie akceptujemy automatyczne tworzenie schematu.
// W rzeczywistych systemach używamy migracji wdrażanych kontrolowanie.
if (app.Configuration.GetValue<bool>("Lab:Initialize"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<LabDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DemoSeeder.SeedAsync(db);
}

app.MapGet("/health", () => Results.Ok(new { status = "ready" }));

var api = app.MapGroup("/api/orders");
api.MapGet("/nplusone", async (LabDbContext db, SqlCommandCounter counter, int take = 100, CancellationToken ct = default) =>
{
    if (take is < 1 or > 500) return Results.BadRequest(new { error = "take: 1..500" });
    using var measurement = counter.Begin();
    var sw = Stopwatch.StartNew();
    var orders = await db.Orders.AsNoTracking().OrderBy(x => x.Id).Take(take)
        .TagWith("EFPLAB:NPLUSONE:ORDERS")
        .Select(x => new { x.Id, x.CustomerName }).ToListAsync(ct);

    // Antywzorzec: jawnie jedno dodatkowe SELECT na każde zamówienie.
    // Nie wymaga włączania lazy loading proxies; pokazuje to samo N+1.
    var output = new List<OrderDto>(orders.Count);
    foreach (var order in orders)
    {
        var items = await db.OrderItems.AsNoTracking().Where(x => x.OrderId == order.Id)
            .OrderBy(x => x.Id).TagWith("EFPLAB:NPLUSONE:ITEMS")
            .Select(x => new OrderItemDto(x.Id, x.ProductName, x.Price, x.Quantity))
            .ToListAsync(ct);
        output.Add(new OrderDto(order.Id, order.CustomerName, items));
    }
    sw.Stop();
    return Results.Ok(new MeasuredResponse("nplusone", output.Count, measurement.Count, sw.Elapsed.TotalMilliseconds, output));
});

api.MapGet("/include", async (LabDbContext db, SqlCommandCounter counter, int take = 100, CancellationToken ct = default) =>
{
    if (take is < 1 or > 500) return Results.BadRequest(new { error = "take: 1..500" });
    using var measurement = counter.Begin();
    var sw = Stopwatch.StartNew();
    var orders = await db.Orders.AsNoTracking().AsSingleQuery()
        .OrderBy(x => x.Id).Take(take).Include(x => x.Items)
        .TagWith("EFPLAB:INCLUDE").ToListAsync(ct);
    var output = orders.Select(x => new OrderDto(x.Id, x.CustomerName,
        x.Items.OrderBy(i => i.Id).Select(i => new OrderItemDto(i.Id, i.ProductName, i.Price, i.Quantity)).ToList()))
        .ToList();
    sw.Stop();
    return Results.Ok(new MeasuredResponse("include", output.Count, measurement.Count, sw.Elapsed.TotalMilliseconds, output));
});

api.MapGet("/projection", async (LabDbContext db, SqlCommandCounter counter, int take = 100, CancellationToken ct = default) =>
{
    if (take is < 1 or > 500) return Results.BadRequest(new { error = "take: 1..500" });
    using var measurement = counter.Begin();
    var sw = Stopwatch.StartNew();
    var output = await db.Orders.AsNoTracking().AsSingleQuery().OrderBy(x => x.Id).Take(take)
        .TagWith("EFPLAB:PROJECTION")
        .Select(x => new OrderDto(x.Id, x.CustomerName,
            x.Items.OrderBy(i => i.Id)
                .Select(i => new OrderItemDto(i.Id, i.ProductName, i.Price, i.Quantity)).ToList()))
        .ToListAsync(ct);
    sw.Stop();
    return Results.Ok(new MeasuredResponse("projection", output.Count, measurement.Count, sw.Elapsed.TotalMilliseconds, output));
});

api.MapGet("/split", async (LabDbContext db, SqlCommandCounter counter, int take = 100, CancellationToken ct = default) =>
{
    if (take is < 1 or > 500) return Results.BadRequest(new { error = "take: 1..500" });
    using var measurement = counter.Begin();
    var sw = Stopwatch.StartNew();
    var orders = await db.Orders.AsNoTracking().AsSplitQuery()
        .OrderBy(x => x.Id).Take(take).Include(x => x.Items)
        .TagWith("EFPLAB:SPLIT").ToListAsync(ct);
    var output = orders.Select(x => new OrderDto(x.Id, x.CustomerName,
        x.Items.OrderBy(i => i.Id).Select(i => new OrderItemDto(i.Id, i.ProductName, i.Price, i.Quantity)).ToList()))
        .ToList();
    sw.Stop();
    return Results.Ok(new MeasuredResponse("split", output.Count, measurement.Count, sw.Elapsed.TotalMilliseconds, output));
});

app.Run();

public sealed record OrderItemDto(int Id, string ProductName, decimal Price, int Quantity);
public sealed record OrderDto(int Id, string CustomerName, List<OrderItemDto> Items);
public sealed record MeasuredResponse(string Strategy, int Orders, int SqlCommands, double ElapsedMs, List<OrderDto> Data);

public sealed class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public List<OrderItem> Items { get; set; } = [];
}
public sealed class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string ProductName { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
public sealed class LabDbContext(DbContextOptions<LabDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("Orders");
            b.HasKey(x => x.Id);
            b.Property(x => x.CustomerName).IsRequired().HasMaxLength(120);
            b.HasMany(x => x.Items).WithOne(x => x.Order).HasForeignKey(x => x.OrderId);
        });
        modelBuilder.Entity<OrderItem>(b =>
        {
            b.ToTable("OrderItems");
            b.HasKey(x => x.Id);
            b.Property(x => x.ProductName).IsRequired().HasMaxLength(120);
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.HasIndex(x => x.OrderId);
        });
    }
}
public static class DemoSeeder
{
    public static async Task SeedAsync(LabDbContext db)
    {
        if (await db.Orders.AnyAsync()) return;
        // Stała liczba i kolejność danych: 100 zamówień × 10 pozycji.
        var orders = Enumerable.Range(1, 100).Select(i => new Order
        {
            CustomerName = $"Klient {i:000}",
            Items = Enumerable.Range(1, 10).Select(j => new OrderItem
            {
                ProductName = $"Produkt {j:00}",
                Price = (i + j) * 1.25m,
                Quantity = j
            }).ToList()
        }).ToList();
        db.Orders.AddRange(orders);
        await db.SaveChangesAsync();
    }
}

// Interceptor mierzy liczbę poleceń SELECT/reader per request, nie wszystkie SQL w serwerze.
public sealed class SqlCommandCounter : DbCommandInterceptor
{
    private readonly AsyncLocal<CounterState?> _active = new();
    public Measurement Begin()
    {
        var previous = _active.Value;
        var current = new CounterState();
        _active.Value = current;
        return new Measurement(this, current, previous);
    }
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        if (_active.Value is { } state) Interlocked.Increment(ref state.Commands);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }
    internal sealed class CounterState { public int Commands; }
    public sealed class Measurement : IDisposable
    {
        private readonly SqlCommandCounter _owner;
        private readonly CounterState _current;
        private readonly CounterState? _previous;
        private bool _disposed;
        internal Measurement(SqlCommandCounter owner, CounterState current, CounterState? previous)
        { _owner = owner; _current = current; _previous = previous; }
        public int Count => Volatile.Read(ref _current.Commands);
        public void Dispose()
        {
            if (_disposed) return;
            _owner._active.Value = _previous;
            _disposed = true;
        }
    }
}
