namespace Demo14;

public interface IEntity<TId>
{
    TId Id { get; }
}

public sealed class Product : IEntity<string>, IEquatable<Product>
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }

    public bool Equals(Product? other) => other is not null && Id == other.Id;
    public override bool Equals(object? obj) => Equals(obj as Product);
    public override int GetHashCode() => Id.GetHashCode(StringComparison.Ordinal);
}

public sealed class Repository<T, TId> where T : class, IEntity<TId> where TId : notnull
{
    private readonly Dictionary<TId, T> _items = new();

    public void Add(T item) => _items.Add(item.Id, item);

    public T Get(TId id) => _items.TryGetValue(id, out var item)
        ? item
        : throw new KeyNotFoundException(id.ToString());

    public IReadOnlyCollection<T> All() => _items.Values;
    public int Count => _items.Count;
}

public static class Program
{
    public static int Main()
    {
        var repo = new Repository<Product, string>();
        repo.Add(new Product { Id = "SKU-1", Name = "Notes", Price = 9.9m });
        Console.WriteLine(repo.Get("SKU-1").Name);
        return 0;
    }
}
