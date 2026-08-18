using Demo14;
using Xunit;

namespace Demo14.Tests;

public class RepositoryTests
{
    [Fact]
    public void Add_Then_Get()
    {
        var repo = new Repository<Product, string>();
        repo.Add(new Product { Id = "A", Name = "Pen", Price = 2m });
        Assert.Equal("Pen", repo.Get("A").Name);
        Assert.Equal(1, repo.Count);
    }

    [Fact]
    public void DuplicateId_Throws()
    {
        var repo = new Repository<Product, string>();
        repo.Add(new Product { Id = "A", Name = "Pen", Price = 2m });
        Assert.Throws<ArgumentException>(() =>
            repo.Add(new Product { Id = "A", Name = "Other", Price = 1m }));
    }

    [Fact]
    public void Products_Equal_ById()
    {
        var a = new Product { Id = "A", Name = "X", Price = 1m };
        var b = new Product { Id = "A", Name = "Y", Price = 2m };
        Assert.Equal(a, b);
    }
}
