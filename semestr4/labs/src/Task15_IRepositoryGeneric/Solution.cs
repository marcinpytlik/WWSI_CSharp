namespace Task15_IRepositoryGeneric;

public interface IRepository<T, in TKey>
{
    void Add(T item);
    bool Remove(TKey id);
    T? GetById(TKey id);
    IReadOnlyList<T> GetAll();
}

public sealed class InMemoryRepository<T, TKey> : IRepository<T, TKey>
    where TKey : notnull
{
    private readonly Dictionary<TKey, T> _items = new();
    private readonly Func<T, TKey> _key;

    public InMemoryRepository(Func<T, TKey> keySelector) => _key = keySelector;

    public void Add(T item) => _items[_key(item)] = item;

    public bool Remove(TKey id) => _items.Remove(id);

    public T? GetById(TKey id) => _items.TryGetValue(id, out var v) ? v : default;

    public IReadOnlyList<T> GetAll() => _items.Values.ToList();
}
