using Microsoft.EntityFrameworkCore;

namespace Demo11;

public sealed class SqlOrderStore : IOrderStore
{
    private readonly OrdersDbContext _db;

    public SqlOrderStore(OrdersDbContext db) => _db = db;

    public async Task AddAsync(OrderRecord order, CancellationToken cancellationToken = default)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<OrderRecord?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
