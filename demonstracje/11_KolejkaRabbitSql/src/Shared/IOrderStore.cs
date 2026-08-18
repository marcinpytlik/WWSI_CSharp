namespace Demo11;

public interface IOrderStore
{
    Task AddAsync(OrderRecord order, CancellationToken cancellationToken = default);
    Task<OrderRecord?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
