using Microsoft.EntityFrameworkCore;

namespace QueueCacheDemo;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
public sealed class ProcessedCommand
{
    public Guid Id { get; set; }
    public DateTime CompletedAtUtc { get; set; }
}
public sealed record CreateProduct(Guid OperationId, Guid ProductId, string Name, decimal Price);
public sealed record ProductDto(Guid Id, string Name, decimal Price);

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProcessedCommand> ProcessedCommands => Set<ProcessedCommand>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>().HasKey(p => p.Id);
        b.Entity<Product>().Property(p => p.Name).HasMaxLength(200).IsRequired();
        b.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        b.Entity<ProcessedCommand>().HasKey(x => x.Id);
    }
}
