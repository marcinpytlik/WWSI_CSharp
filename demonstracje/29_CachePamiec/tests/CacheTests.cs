using Demo29;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Demo29.Tests;

public sealed class CountingSource : IProductSource
{
    public int Calls { get; private set; }
    public Product Get(string sku)
    {
        Calls++;
        return new Product(sku, "Notes", 9.9m);
    }
}

public class CacheTests
{
    [Fact]
    public void SecondGet_DoesNotHitSource()
    {
        var source = new CountingSource();
        var catalog = new CachedCatalog(source, new MemoryCache(new MemoryCacheOptions()));
        catalog.Get("SKU-1");
        catalog.Get("SKU-1");
        Assert.Equal(1, source.Calls);
    }

    [Fact]
    public void DifferentSku_HitsSourceAgain()
    {
        var source = new CountingSource();
        var catalog = new CachedCatalog(source, new MemoryCache(new MemoryCacheOptions()));
        catalog.Get("A");
        catalog.Get("B");
        Assert.Equal(2, source.Calls);
    }
}
