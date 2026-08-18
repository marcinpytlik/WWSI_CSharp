using System.Linq.Expressions;

namespace Demo63;

public sealed class Order
{
    public int Id { get; init; }
    public string Customer { get; init; } = "";
    public decimal Total { get; init; }
    public bool Paid { get; init; }
}

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    Expression<Func<T, object>>? OrderBy { get; }
}

public sealed class PaidOrdersSpec : ISpecification<Order>
{
    public Expression<Func<Order, bool>> Criteria => o => o.Paid;
    public Expression<Func<Order, object>>? OrderBy => o => o.Total;
}

public sealed class CustomerSpec : ISpecification<Order>
{
    private readonly string _customer;
    public CustomerSpec(string customer) => _customer = customer;
    public Expression<Func<Order, bool>> Criteria => o => o.Customer == _customer;
    public Expression<Func<Order, object>>? OrderBy => o => o.Id;
}

public static class SpecEvaluator
{
    public static IReadOnlyList<T> Apply<T>(IEnumerable<T> source, ISpecification<T> spec)
    {
        var query = source.AsQueryable().Where(spec.Criteria);
        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);
        return query.ToList();
    }
}

public static class Program
{
    public static int Main()
    {
        var orders = new[]
        {
            new Order { Id = 1, Customer = "Ada", Total = 10, Paid = true },
            new Order { Id = 2, Customer = "Ada", Total = 50, Paid = false }
        };
        Console.WriteLine(SpecEvaluator.Apply(orders, new PaidOrdersSpec()).Count);
        return 0;
    }
}
