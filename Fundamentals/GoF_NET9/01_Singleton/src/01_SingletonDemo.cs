namespace GoF.Singleton;

public sealed class AppCache
{
    private static readonly Lazy<AppCache> _i = new(() => new AppCache());
    public static AppCache Instance => _i.Value;
    private AppCache() {}
    public int Counter { get; private set; }
    public void Inc() => Counter++;
}
