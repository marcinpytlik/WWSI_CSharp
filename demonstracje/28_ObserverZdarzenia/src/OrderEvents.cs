namespace Demo28;

public sealed record OrderPlaced(string Sku, int Qty, DateTime At);

public sealed class OrderDesk
{
    public event Action<OrderPlaced>? Placed;

    public void Place(string sku, int qty)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        Placed?.Invoke(new OrderPlaced(sku.Trim(), qty, DateTime.UtcNow));
    }
}

public sealed class EmailListener
{
    public List<string> Inbox { get; } = [];
    public void OnPlaced(OrderPlaced e) => Inbox.Add($"{e.Sku} x{e.Qty}");
}

public static class Program
{
    public static int Main()
    {
        var desk = new OrderDesk();
        var mail = new EmailListener();
        desk.Placed += mail.OnPlaced;
        desk.Place("SKU-1", 2);
        Console.WriteLine(mail.Inbox.Single());
        return 0;
    }
}
