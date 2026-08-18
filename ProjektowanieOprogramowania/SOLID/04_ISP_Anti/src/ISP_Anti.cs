namespace Solid.ISP.Anti;

public interface IRepository<T>
{
    T? Get(int id);
    IEnumerable<T> All();
    void Add(T entity);
    void BulkInsert(IEnumerable<T> items);
    void Truncate(); // nadmiarowe dla większości klientów
}

public class SimpleRepo : IRepository<int>
{
    public int? Get(int id) => id;
    public IEnumerable<int> All() => new[] { 1, 2, 3 };
    public void Add(int entity) { /* ... */ }
    public void BulkInsert(IEnumerable<int> items) => throw new NotImplementedException(); // ISP issue
    public void Truncate() => throw new NotImplementedException(); // ISP issue
}
