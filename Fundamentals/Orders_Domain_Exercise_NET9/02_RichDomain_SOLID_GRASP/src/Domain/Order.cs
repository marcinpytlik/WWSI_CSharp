
using Orders.Rich.Domain.Primitives;

namespace Orders.Rich.Domain;

public enum OrderStatus { New, Paid, Cancelled }

public sealed class Order
{
    private readonly List<OrderLine> _lines = new();
    public Guid Id { get; } = Guid.NewGuid();
    public Email CustomerEmail { get; }
    public DateTime CreatedUtc { get; }
    public OrderStatus Status { get; private set; } = OrderStatus.New;

    private Order(Email email, DateTime createdUtc) => (CustomerEmail, CreatedUtc) = (email, createdUtc);

    public static Order Create(Email email, DateTime createdUtc) => new(email, createdUtc);

    public void AddLine(string sku, int qty, Money unitPrice)
    {
        if (qty <= 0 || unitPrice.Amount <= 0) throw new ArgumentException("Invalid line");
        _lines.Add(new OrderLine(sku, qty, unitPrice));
    }

    public Money TotalNet() => _lines.Select(l => l.Total()).DefaultIfEmpty(Money.Zero()).Aggregate((a,b) => a + b);

    public void Pay(IClock clock, IEmailSender email, IPricingStrategy pricing, IVatPolicy vat)
    {
        if (Status != OrderStatus.New) throw new InvalidOperationException("Invalid status");
        var net = TotalNet();
        var priced = pricing.Apply(net);
        var gross = vat.Apply(priced).Round2();
        Status = OrderStatus.Paid;
        email.SendAsync(CustomerEmail.Value, "Order paid", $"Amount: {gross}").GetAwaiter().GetResult();
    }
}

public sealed class OrderLine
{
    public string Sku { get; }
    public int Qty { get; }
    public Money UnitPrice { get; }
    public OrderLine(string sku, int qty, Money unitPrice) => (Sku, Qty, UnitPrice) = (sku, qty, unitPrice);
    public Money Total() => UnitPrice * Qty;
}
