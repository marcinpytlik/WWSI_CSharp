namespace Task14_InMemoryProductRepo;

public sealed record Product(int Id, string Name, decimal Price);

public sealed class InMemoryProductRepository
{
    private readonly Dictionary<int, Product> _items = new();

    public void Add(Product product) => _items[product.Id] = product;

    public bool Remove(int id) => _items.Remove(id);

    public Product? GetById(int id) => _items.TryGetValue(id, out var p) ? p : null;

    public IReadOnlyList<Product> GetAll() => _items.Values.OrderBy(p => p.Id).ToList();
}
