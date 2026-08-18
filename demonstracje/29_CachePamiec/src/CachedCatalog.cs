using Microsoft.Extensions.Caching.Memory;

namespace Demo29;

public sealed record Product(string Sku, string Name, decimal Price);

public interface IProductSource
{
    Product Get(string sku);
}

public sealed class CachedCatalog : IProductSource
{
    private readonly IProductSource _inner;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _ttl;

    public CachedCatalog(IProductSource inner, IMemoryCache cache, TimeSpan? ttl = null)
    {
        _inner = inner;
        _cache = cache;
        _ttl = ttl ?? TimeSpan.FromMinutes(5);
    }

    public Product Get(string sku)
        => _cache.GetOrCreate(sku, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _ttl;
            return _inner.Get(sku);
        })!;
}

public static class Program
{
    public static int Main()
    {
        var catalog = new CachedCatalog(new StaticSource(), new MemoryCache(new MemoryCacheOptions()));
        Console.WriteLine(catalog.Get("SKU-1").Name);
        return 0;
    }
}

file sealed class StaticSource : IProductSource
{
    public Product Get(string sku) => new(sku, "Notes", 9.9m);
}
