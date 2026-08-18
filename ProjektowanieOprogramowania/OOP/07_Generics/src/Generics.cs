namespace Oop.Generics;

public interface IRepository<T>
{
    T? Get(int id);
    void Add(int id, T entity);
}

public class InMemoryRepository<T> : IRepository<T>
{
    private readonly Dictionary<int, T> _db = new();
    public T? Get(int id) => _db.TryGetValue(id, out var v) ? v : default;
    public void Add(int id, T entity) => _db[id] = entity;
}
