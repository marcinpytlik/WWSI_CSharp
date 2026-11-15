using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

var builder = WebApplication.CreateBuilder(args);

// Ćwiczenia 1 i 5 – DI + provider; ustawienia pobierane z konfiguracji środowiska.
// Brak connection string w Production skutkuje czytelnym błędem startu.
string connectionString = builder.Configuration.GetConnectionString("Products")
    ?? throw new InvalidOperationException(
        "Brak ConnectionStrings:Products. Ustaw User Secrets lub zmienną środowiskową " +
        "ConnectionStrings__Products. Patrz solution/README.md.");

builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Ćwiczenia 3 i 4: migracja oraz deterministyczny seed wyłącznie w Development.
// W produkcji migrację należy wykonać kontrolowanym procesem wdrożeniowym.
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    await db.Database.MigrateAsync();
    await DevelopmentSeeder.SeedAsync(db);
}

app.MapGet("/api/v1/products", async (ProductsDbContext db, CancellationToken ct) =>
    await db.Products.AsNoTracking().OrderBy(p => p.Id)
        .Select(p => new ProductResponse(p.Id, p.Sku, p.Name, p.Price))
        .ToListAsync(ct));

app.MapGet("/api/v1/products/{id:int}", async (int id, ProductsDbContext db,
    CancellationToken ct) =>
{
    var product = await db.Products.AsNoTracking()
        .Where(p => p.Id == id)
        .Select(p => new ProductResponse(p.Id, p.Sku, p.Name, p.Price))
        .FirstOrDefaultAsync(ct);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.Run();

// Ćwiczenie 1 – encja i DbSet<Product>.
public sealed class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

public sealed record ProductResponse(int Id, string Sku, string Name, decimal Price);

public sealed class ProductsDbContext(DbContextOptions<ProductsDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}

// Ćwiczenie 2 – reguły Fluent API (klucz, długości, wymagane pola, precyzja, unikalność).
public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.Sku).HasMaxLength(32).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Price).HasPrecision(18, 2).IsRequired();
        builder.HasIndex(p => p.Sku).IsUnique();
    }
}

// Ćwiczenie 4 – bez HasData: dane demonstracyjne wyłącznie w Development.
// Stałe identyfikatory biznesowe (SKU) i warunek istnienia zapobiegają duplikatom.
public static class DevelopmentSeeder
{
    public static async Task SeedAsync(ProductsDbContext db, CancellationToken ct = default)
    {
        (string Sku, string Name, decimal Price)[] products =
        [
            ("WWSI-001", "Kurs C# i .NET 10", 199.00m),
            ("WWSI-002", "Kurs ASP.NET Core", 249.00m),
            ("WWSI-003", "Kurs EF Core", 279.00m)
        ];

        // Transakcja ujednolica zapis; unikalny indeks SKU stanowi zabezpieczenie w DB.
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        foreach (var item in products)
        {
            if (!await db.Products.AnyAsync(p => p.Sku == item.Sku, ct))
                db.Products.Add(new Product
                {
                    Sku = item.Sku, Name = item.Name, Price = item.Price
                });
        }
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
}
