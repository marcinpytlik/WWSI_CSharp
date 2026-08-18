namespace Demo26;

public enum OrderState { New, Paid, Shipped, Cancelled }

public sealed class Order
{
    public Order(string sku, int qty)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        Sku = sku.Trim();
        Qty = qty;
        State = OrderState.New;
    }

    public string Sku { get; }
    public int Qty { get; }
    public OrderState State { get; private set; }

    public void Pay() => MoveTo(OrderState.Paid, OrderState.New);
    public void Ship() => MoveTo(OrderState.Shipped, OrderState.Paid);

    public void Cancel()
    {
        if (State == OrderState.Shipped)
            throw new InvalidOperationException("Cannot cancel a shipped order.");
        State = OrderState.Cancelled;
    }

    private void MoveTo(OrderState next, OrderState required)
    {
        if (State != required)
            throw new InvalidOperationException($"Cannot move {State} -> {next}.");
        State = next;
    }
}

public static class Program
{
    public static int Main()
    {
        var order = new Order("SKU-1", 1);
        order.Pay();
        order.Ship();
        Console.WriteLine(order.State);
        return 0;
    }
}
