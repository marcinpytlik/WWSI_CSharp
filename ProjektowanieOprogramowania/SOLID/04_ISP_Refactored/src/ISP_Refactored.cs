namespace Solid.ISP.Ref;

public interface IReader<T> { T? Get(int id); IEnumerable<T> All(); }
public interface IWriter<T> { void Add(T entity); }
public interface IAdminOps { void Truncate(); }

public class SimpleReader : IReader<int>
{
    public int? Get(int id) => id;
    public IEnumerable<int> All() => new[] { 1, 2, 3 };
}
