namespace Demo65;

public interface IDiscount
{
    string Name { get; }
    decimal Apply(decimal price);
}

public sealed class PercentDiscount : IDiscount
{
    private readonly decimal _percent;
    public PercentDiscount(decimal percent) => _percent = percent;
    public string Name => $"{_percent:0}%";
    public decimal Apply(decimal price) => price * (1 - _percent / 100m);
}

public sealed class NoDiscount : IDiscount
{
    public static NoDiscount Instance { get; } = new();
    public string Name => "none";
    public decimal Apply(decimal price) => price;
}

public sealed class Checkout
{
    private readonly IDiscount _discount;
    public Checkout(IDiscount discount) => _discount = discount;

    public decimal Total(decimal price)
    {
        if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));
        return _discount.Apply(price);
    }
}

public static class Program
{
    public static int Main()
    {
        Console.WriteLine(new Checkout(NoDiscount.Instance).Total(100));
        return 0;
    }
}
