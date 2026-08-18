namespace Oop.Composition;

public interface IDiscount { decimal Apply(decimal price); }
public class Percent10 : IDiscount { public decimal Apply(decimal p) => p * 0.9m; }

public class Report  // kompozycja
{
    private readonly IDiscount _discount;
    public Report(IDiscount discount) => _discount = discount;
    public decimal Price(decimal p) => _discount.Apply(p);
}
