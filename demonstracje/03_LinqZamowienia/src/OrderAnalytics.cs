namespace Demo03;

public sealed record Order(string Customer, string Product, int Qty, decimal UnitPrice)
{
    public decimal LineTotal => Qty * UnitPrice;
}

public static class OrderAnalytics
{
    public static decimal TotalFor(IEnumerable<Order> orders, string customer)
        => orders.Where(o => o.Customer == customer).Sum(o => o.LineTotal);

    public static IReadOnlyList<(string Product, int Qty)> TopProducts(IEnumerable<Order> orders, int take)
        => orders
            .GroupBy(o => o.Product)
            .Select(g => (Product: g.Key, Qty: g.Sum(x => x.Qty)))
            .OrderByDescending(x => x.Qty)
            .Take(take)
            .ToList();

    public static IReadOnlyList<Order> Expensive(IEnumerable<Order> orders, decimal minLine)
        => orders.Where(o => o.LineTotal >= minLine).ToList();
}

public static class Program
{
    public static int Main()
    {
        Order[] data =
        [
            new("Ada", "Book", 2, 40),
            new("Ada", "Pen", 10, 2),
            new("Jan", "Book", 1, 40)
        ];
        Console.WriteLine($"Ada total: {OrderAnalytics.TotalFor(data, "Ada")}");
        foreach (var (product, qty) in OrderAnalytics.TopProducts(data, 2))
            Console.WriteLine($"{product}: {qty}");
        return 0;
    }
}
