using Demo12;
using Microsoft.EntityFrameworkCore;

namespace Demo12.DatabaseFirst;

public sealed class EfProductStore : IProductStore
{
    private readonly CatalogDbContext _db;

    public EfProductStore(CatalogDbContext db) => _db = db;

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.Products.AsNoTracking().OrderBy(x => x.Sku).ToListAsync(cancellationToken);
}
