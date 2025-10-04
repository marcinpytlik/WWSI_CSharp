namespace Oop.Interfaces;

public interface IDiscount { decimal Apply(decimal price); }
public class NoDiscount : IDiscount { public decimal Apply(decimal p) => p; }
public class Percent10 : IDiscount { public decimal Apply(decimal p) => p * 0.9m; }

public class Checkout
{
    private readonly IDiscount _discount;
    public Checkout(IDiscount discount) => _discount = discount;
    public decimal Total(decimal price) => _discount.Apply(price);
}
