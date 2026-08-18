namespace Demo12;

public sealed class ProductService
{
    private readonly IProductStore _store;

    public ProductService(IProductStore store) => _store = store;

    public async Task<Product> AddAsync(string sku, string name, decimal price, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU is required.", nameof(sku));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        var product = new Product
        {
            Sku = sku.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Price = decimal.Round(price, 2, MidpointRounding.AwayFromZero)
        };
        await _store.AddAsync(product, cancellationToken);
        return product;
    }

    public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default)
        => _store.ListAsync(cancellationToken);
}
