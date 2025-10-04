namespace GoF.Strategy;

public interface IDiscount { decimal Apply(decimal price); }
public class Percentage : IDiscount { public decimal Apply(decimal p)=> p*0.9m; }
public class NoDiscount : IDiscount { public decimal Apply(decimal p)=> p; }

public class Checkout
{
    private readonly IDiscount _discount;
    public Checkout(IDiscount d)=> _discount=d;
    public decimal Pay(decimal price)=> _discount.Apply(price);
}
