namespace Demo12;

public interface IProductStore
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default);
}
