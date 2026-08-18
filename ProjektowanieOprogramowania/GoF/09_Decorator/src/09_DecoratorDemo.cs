namespace GoF.Decorator;

public interface IData { Task<string> GetAsync(string k); }
public class Data : IData { public Task<string> GetAsync(string k)=> Task.FromResult(k.ToUpperInvariant()); }

public class DataWithCache : IData
{
    private readonly IData _inner; private readonly Dictionary<string,string> _cache = new();
    public DataWithCache(IData inner)=>_inner=inner;
    public async Task<string> GetAsync(string k)
    {
        if(_cache.TryGetValue(k,out var v)) return v;
        var r = await _inner.GetAsync(k); _cache[k]=r; return r;
    }
}
