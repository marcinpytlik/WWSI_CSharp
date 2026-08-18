namespace Task22_MoqRepositoryTarget;

public interface IRepository<T>
{
    IReadOnlyList<T> GetAll();
}

public sealed record Product(int Id, string Name, decimal Price);

public sealed class OrderService
{
    private readonly IRepository<Product> _repo;

    public OrderService(IRepository<Product> repo) => _repo = repo;

    public decimal Total() => _repo.GetAll().Sum(p => p.Price);
}
