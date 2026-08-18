namespace Task23_OrderServiceTotal;

public sealed record Product(int Id, string Name, decimal Price);

public sealed class OrderService
{
    public decimal CalculateTotal(IEnumerable<Product> products)
        => products.Sum(p => p.Price);
}
