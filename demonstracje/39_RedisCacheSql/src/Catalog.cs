using Microsoft.EntityFrameworkCore;

namespace Demo39;

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public sealed class CatalogDb : DbContext
{
    public CatalogDb(DbContextOptions<CatalogDb> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(p => p.Name).HasMaxLength(80).IsRequired();
        });
    }
}
