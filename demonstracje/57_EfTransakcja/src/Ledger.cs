using Microsoft.EntityFrameworkCore;

namespace Demo57;

public sealed class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Balance { get; set; }
}

public sealed class LedgerDb : DbContext
{
    public LedgerDb(DbContextOptions<LedgerDb> options) : base(options) { }
    public DbSet<Account> Accounts => Set<Account>();
}

public sealed class TransferService
{
    private readonly LedgerDb _db;
    public TransferService(LedgerDb db) => _db = db;

    public async Task TransferAsync(int fromId, int toId, decimal amount, bool explodeAfterDebit)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();
        var from = await _db.Accounts.FirstAsync(a => a.Id == fromId);
        var to = await _db.Accounts.FirstAsync(a => a.Id == toId);
        from.Balance -= amount;
        await _db.SaveChangesAsync();
        if (explodeAfterDebit)
            throw new InvalidOperationException("Simulated failure after debit.");
        to.Balance += amount;
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}

public static class Program
{
    public static async Task<int> Main()
    {
        var options = new DbContextOptionsBuilder<LedgerDb>().UseSqlite("Data Source=demo57.db").Options;
        await using var db = new LedgerDb(options);
        await db.Database.EnsureCreatedAsync();
        Console.WriteLine("TransferService uses IDbContextTransaction. See tests.");
        return 0;
    }
}
