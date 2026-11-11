using Microsoft.EntityFrameworkCore;
using QueueCacheDemo.Entities;

namespace QueueCacheDemo.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProcessedCommand> ProcessedCommands => Set<ProcessedCommand>();
    public DbSet<OperationStatus> OperationStatuses => Set<OperationStatus>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
