namespace Grasp.Refactored.Domain;

public sealed record OrderLine(string Sku, int Qty, decimal UnitPrice)
{
    public decimal LineTotal() => Qty * UnitPrice;
}

public sealed class Order
{
    private readonly List<OrderLine> _lines = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Email { get; }
    public DateTime CreatedUtc { get; }

    public Order(string email, DateTime createdUtc)
    {
        Email = email;
        CreatedUtc = createdUtc;
    }

    public IReadOnlyList<OrderLine> Lines => _lines;

    public void AddLine(OrderLine line) => _lines.Add(line);

    public decimal BaseTotal() => _lines.Sum(l => l.LineTotal());
}
