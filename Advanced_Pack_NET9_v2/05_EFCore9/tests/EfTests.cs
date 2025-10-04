
using Microsoft.EntityFrameworkCore;
using Advanced.EfCore9;
using Xunit;

public class EfTests
{
    [Fact]
    public async Task Can_Insert_And_Query_InMemory()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("test").Options;
        await using var db = new AppDbContext(opts);
        db.Products.Add(new Product{ Name="A", Price=1.23m });
        await db.SaveChangesAsync();

        var p = await db.Products.AsNoTracking().FirstAsync();
        Assert.Equal("A", p.Name);
    }
}
