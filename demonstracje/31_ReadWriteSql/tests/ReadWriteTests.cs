using Demo31;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Demo31.Tests;

public class ReadWriteTests
{
    [Fact]
    public async Task Writer_Persists_Reader_SeesRow()
    {
        var path = Path.Combine(Path.GetTempPath(), $"demo31-{Guid.NewGuid():N}.db");
        var writeOpt = new DbContextOptionsBuilder<WriteCatalogDb>().UseSqlite($"Data Source={path}").Options;
        var readOpt = new DbContextOptionsBuilder<ReadCatalogDb>().UseSqlite($"Data Source={path}").Options;
        await using (var write = new WriteCatalogDb(writeOpt))
        {
            await write.Database.EnsureCreatedAsync();
            await new CatalogWriter(write).AddAsync("sku-1", "Notes");
        }

        await using var read = new ReadCatalogDb(readOpt);
        var all = await new CatalogReader(read).ListAsync();
        Assert.Equal("SKU-1", all.Single().Sku);
    }

    [Fact]
    public void Accounts_AreDistinct()
    {
        Assert.NotEqual(SqlAccounts.WriteUser, SqlAccounts.ReadUser);
        Assert.Equal("demo31_write", SqlAccounts.WriteUser);
    }
}
