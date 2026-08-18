namespace Demo27;

public interface IPriceStep
{
    decimal Apply(decimal net);
}

public sealed class BasePrice : IPriceStep
{
    public decimal Apply(decimal net) => net;
}

public abstract class PriceDecorator : IPriceStep
{
    protected PriceDecorator(IPriceStep inner) => Inner = inner;
    protected IPriceStep Inner { get; }
    public abstract decimal Apply(decimal net);
}

public sealed class VatDecorator : PriceDecorator
{
    private readonly decimal _rate;
    public VatDecorator(IPriceStep inner, decimal rate = 1.23m) : base(inner) => _rate = rate;
    public override decimal Apply(decimal net) => Math.Round(Inner.Apply(net) * _rate, 2, MidpointRounding.AwayFromZero);
}

public sealed class DiscountDecorator : PriceDecorator
{
    private readonly decimal _percent;
    public DiscountDecorator(IPriceStep inner, decimal percent) : base(inner)
    {
        if (percent is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(percent));
        _percent = percent;
    }

    public override decimal Apply(decimal net)
        => Math.Round(Inner.Apply(net) * (1 - _percent), 2, MidpointRounding.AwayFromZero);
}

public sealed class ShippingDecorator : PriceDecorator
{
    private readonly decimal _fee;
    public ShippingDecorator(IPriceStep inner, decimal fee) : base(inner) => _fee = fee;
    public override decimal Apply(decimal net) => Inner.Apply(net) + _fee;
}

public static class Program
{
    public static int Main()
    {
        IPriceStep pipeline = new ShippingDecorator(new VatDecorator(new DiscountDecorator(new BasePrice(), 0.10m)), 12m);
        Console.WriteLine(pipeline.Apply(100m));
        return 0;
    }
}
