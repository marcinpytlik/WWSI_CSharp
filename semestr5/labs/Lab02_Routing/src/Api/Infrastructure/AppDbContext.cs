using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.HasDefaultSchema("dbo");

        mb.Entity<Product>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.HasIndex(x => x.Sku).IsUnique(false);
        });

        mb.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Username).HasMaxLength(100).IsRequired();
        });
    }
}
