using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QueueCacheDemo.Data;
using QueueCacheDemo.Entities;
using Xunit;

public sealed class ModelTests
{
    [Fact]
    public async Task OperationKeyIsUniquePerPrincipal()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var now = DateTime.UtcNow;
        db.OperationStatuses.Add(new OperationStatus
        {
            Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), PrincipalId = "student01",
            IdempotencyKey = "same-key", RequestHash = new string('A', 64),
            Status = "Pending", CreatedAtUtc = now, UpdatedAtUtc = now
        });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        db.OperationStatuses.Add(new OperationStatus
        {
            Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), PrincipalId = "student01",
            IdempotencyKey = "same-key", RequestHash = new string('A', 64),
            Status = "Pending", CreatedAtUtc = now, UpdatedAtUtc = now
        });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void EntityConfigurationsMapToExpectedTables()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:").Options;
        using var db = new AppDbContext(options);
        Assert.Equal("Products", db.Model.FindEntityType(typeof(Product))!.GetTableName());
        Assert.Equal("OperationStatuses", db.Model.FindEntityType(typeof(OperationStatus))!.GetTableName());
        Assert.Equal("OutboxMessages", db.Model.FindEntityType(typeof(OutboxMessage))!.GetTableName());
    }
}
