using Demo12;
using Microsoft.EntityFrameworkCore;

namespace Demo12.DatabaseFirst;

/// <summary>
/// Model odwzorowujący tabelę z <c>sql/01_dbfirst_schema.sql</c>.
/// Na sali pokazać analogię do:
/// <c>dotnet ef dbcontext scaffold CONNECTION Microsoft.EntityFrameworkCore.SqlServer --context CatalogDbContext --output-dir Models --force --no-onconfiguring</c>
/// <c>--no-onconfiguring</c> — connection string zostaje poza kodem (konto aplikacji w konfiguracji).
/// </summary>
public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Products");
            entity.ToTable("Products");
            entity.HasIndex(e => e.Sku, "UQ_Products_Sku").IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Sku).HasMaxLength(32);
        });
    }
}
