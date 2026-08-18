namespace Task04_JoinCollections;

public sealed record Customer(int Id, string Name);
public sealed record Order(int OrderId, int CustomerId);

public static class Joiner
{
    public static IReadOnlyList<(string CustomerName, int OrderId)> JoinCustomersOrders(
        IEnumerable<Customer> customers,
        IEnumerable<Order> orders)
        => customers.Join(orders, c => c.Id, o => o.CustomerId, (c, o) => (c.Name, o.OrderId))
                    .ToList();
}
