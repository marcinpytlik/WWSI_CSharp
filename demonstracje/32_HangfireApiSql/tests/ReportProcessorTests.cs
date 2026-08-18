using Demo32;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Demo32.Tests;

public class ReportProcessorTests
{
    private static async Task<ReportsDb> OpenAsync()
    {
        var options = new DbContextOptionsBuilder<ReportsDb>().UseSqlite("DataSource=:memory:").Options;
        var db = new ReportsDb(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();
        return db;
    }

    [Fact]
    public async Task ProcessAsync_MarksDone()
    {
        await using var db = await OpenAsync();
        db.Reports.Add(new Report { Title = "Monthly", Status = "Queued" });
        await db.SaveChangesAsync();

        await new ReportProcessor(db).ProcessAsync(1);

        var saved = await db.Reports.SingleAsync();
        Assert.Equal("Done", saved.Status);
        Assert.NotNull(saved.ProcessedUtc);
    }

    [Fact]
    public async Task MissingReport_Throws()
    {
        await using var db = await OpenAsync();
        await Assert.ThrowsAsync<KeyNotFoundException>(() => new ReportProcessor(db).ProcessAsync(99));
    }
}
