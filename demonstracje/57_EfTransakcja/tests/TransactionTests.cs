using Demo57;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Demo57.Tests;

public class TransactionTests
{
    [Fact]
    public async Task FailureAfterDebit_RollsBackBoth()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<LedgerDb>().UseSqlite(connection).Options;
        await using (var setup = new LedgerDb(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.Accounts.AddRange(
                new Account { Name = "Ada", Balance = 100 },
                new Account { Name = "Alan", Balance = 10 });
            await setup.SaveChangesAsync();
        }

        await using var db = new LedgerDb(options);
        var svc = new TransferService(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.TransferAsync(1, 2, 40, explodeAfterDebit: true));

        var ada = await db.Accounts.AsNoTracking().SingleAsync(a => a.Name == "Ada");
        var alan = await db.Accounts.AsNoTracking().SingleAsync(a => a.Name == "Alan");
        Assert.Equal(100, ada.Balance);
        Assert.Equal(10, alan.Balance);
    }

    [Fact]
    public async Task Success_MovesMoney()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<LedgerDb>().UseSqlite(connection).Options;
        await using (var setup = new LedgerDb(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.Accounts.AddRange(
                new Account { Name = "Ada", Balance = 100 },
                new Account { Name = "Alan", Balance = 10 });
            await setup.SaveChangesAsync();
        }

        await using var db = new LedgerDb(options);
        await new TransferService(db).TransferAsync(1, 2, 40, explodeAfterDebit: false);
        Assert.Equal(60, (await db.Accounts.SingleAsync(a => a.Name == "Ada")).Balance);
        Assert.Equal(50, (await db.Accounts.SingleAsync(a => a.Name == "Alan")).Balance);
    }
}
