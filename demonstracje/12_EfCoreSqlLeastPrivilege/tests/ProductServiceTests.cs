using Demo12;
using Xunit;

namespace Demo12.Tests;

public sealed class MemoryProductStore : IProductStore
{
    private readonly List<Product> _items = [];
    private int _nextId = 1;

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        product.Id = _nextId++;
        _items.Add(product);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Product>>(_items.OrderBy(x => x.Sku).ToList());
}

public class ProductServiceTests
{
    [Fact]
    public async Task Add_Then_List()
    {
        var svc = new ProductService(new MemoryProductStore());
        await svc.AddAsync("sku-42", "Notes", 12.5m);
        var all = await svc.ListAsync();
        Assert.Single(all);
        Assert.Equal("SKU-42", all[0].Sku);
        Assert.Equal("Notes", all[0].Name);
        Assert.Equal(12.50m, all[0].Price);
    }

    [Fact]
    public async Task EmptySku_Throws()
    {
        var svc = new ProductService(new MemoryProductStore());
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AddAsync(" ", "Notes", 1m));
    }

    [Fact]
    public async Task NegativePrice_Throws()
    {
        var svc = new ProductService(new MemoryProductStore());
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => svc.AddAsync("SKU-1", "Notes", -1m));
    }
}
