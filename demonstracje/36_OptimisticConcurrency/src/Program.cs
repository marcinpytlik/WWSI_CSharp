using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("App") ?? "Data Source=demo36.db";
builder.Services.AddDbContext<CatalogDb>(o => o.UseSqlite(cs));
var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDb>();
    await db.Database.EnsureCreatedAsync();
}

app.MapPost("/api/v1/products", async (CreateProduct dto, CatalogDb db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
        return Results.BadRequest(new { error = "Name must have at least 2 characters." });
    var product = new Product { Name = dto.Name.Trim(), Version = 1 };
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/v1/products/{product.Id}", product);
});

app.MapGet("/api/v1/products/{id:int}", async (int id, CatalogDb db) =>
    await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id) is { } product
        ? Results.Ok(product)
        : Results.NotFound());

app.MapPut("/api/v1/products/{id:int}", async (int id, UpdateProduct dto, CatalogDb db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
    if (product is null) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length < 2)
        return Results.BadRequest(new { error = "Name must have at least 2 characters." });
    if (product.Version != dto.Version)
        return Results.Conflict(new { error = "Stale version.", current = product.Version, requested = dto.Version });

    product.Name = dto.Name.Trim();
    product.Version++;
    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        return Results.Conflict(new { error = "Stale version." });
    }

    return Results.Ok(product);
});

app.Run();

public sealed record CreateProduct(string Name);
public sealed record UpdateProduct(string Name, int Version);

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Version { get; set; }
}

public sealed class CatalogDb : DbContext
{
    public CatalogDb(DbContextOptions<CatalogDb> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(80).IsRequired();
            e.Property(p => p.Version).IsConcurrencyToken();
        });
    }
}

public partial class Program;
