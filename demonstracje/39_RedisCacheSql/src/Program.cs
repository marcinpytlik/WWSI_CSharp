using System.Text.Json;
using Demo39;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);
var testing = builder.Configuration.GetValue("Testing", false);

if (testing)
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    var cs = builder.Configuration.GetConnectionString("App")
             ?? throw new InvalidOperationException("Missing ConnectionStrings:App");
    builder.Services.AddDbContext<CatalogDb>(o => o.UseSqlServer(cs));
    var redis = builder.Configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Missing ConnectionStrings:Redis");
    builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redis);
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDb>();
    await SqlRetry.WaitAsync(() => db.Database.EnsureCreatedAsync(), logger);
}

const string cacheKey = "products:all";

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/v1/products", async (CatalogDb db, IDistributedCache cache) =>
{
    var cached = await cache.GetStringAsync(cacheKey);
    if (cached is not null)
        return Results.Ok(JsonSerializer.Deserialize<List<Product>>(cached));

    var items = await db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
    await cache.SetStringAsync(
        cacheKey,
        JsonSerializer.Serialize(items),
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) });
    return Results.Ok(items);
});

app.MapPost("/api/v1/products", async (CreateProduct dto, CatalogDb db, IDistributedCache cache) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
        return Results.BadRequest(new { error = "Name must have at least 2 characters." });
    var product = new Product { Name = dto.Name.Trim() };
    db.Products.Add(product);
    await db.SaveChangesAsync();
    await cache.RemoveAsync(cacheKey);
    return Results.Created($"/api/v1/products/{product.Id}", product);
});

app.Run();

public sealed record CreateProduct(string Name);
public partial class Program;
