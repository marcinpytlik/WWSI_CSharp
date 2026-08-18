using Demo08;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Demo08.Tests;

public class BookServiceTests
{
    [Fact]
    public async Task Add_Then_List()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        await using var db = new LibraryContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();

        var svc = new BookService(db);
        await svc.AddAsync("DDD", 2003);
        var all = await svc.AllAsync();
        Assert.Single(all);
        Assert.Equal("DDD", all[0].Title);
    }

    [Fact]
    public async Task EmptyTitle_Throws()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        await using var db = new LibraryContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();
        await Assert.ThrowsAsync<ArgumentException>(() => new BookService(db).AddAsync(" ", 2000));
    }
}
