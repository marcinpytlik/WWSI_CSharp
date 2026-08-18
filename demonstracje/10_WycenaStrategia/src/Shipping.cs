namespace Demo10;

public interface IShippingStrategy
{
    decimal Cost(decimal orderNet);
}

public sealed class StandardShipping : IShippingStrategy
{
    public decimal Cost(decimal orderNet) => orderNet >= 100m ? 0m : 12m;
}

public sealed class ExpressShipping : IShippingStrategy
{
    public decimal Cost(decimal orderNet) => 25m;
}

public sealed class PickupShipping : IShippingStrategy
{
    public decimal Cost(decimal orderNet) => 0m;
}

public static class ShippingFactory
{
    public static IShippingStrategy Create(string code) => code.Trim().ToLowerInvariant() switch
    {
        "standard" => new StandardShipping(),
        "express" => new ExpressShipping(),
        "pickup" => new PickupShipping(),
        _ => throw new ArgumentException($"Unknown shipping: {code}", nameof(code))
    };
}

public static class Checkout
{
    public static decimal Gross(decimal orderNet, string shippingCode, decimal vat = 1.23m)
    {
        var shipping = ShippingFactory.Create(shippingCode).Cost(orderNet);
        return Math.Round((orderNet + shipping) * vat, 2);
    }
}

public static class Program
{
    public static int Main()
    {
        Console.WriteLine($"standard 80 => {Checkout.Gross(80, "standard")}");
        Console.WriteLine($"standard 120 => {Checkout.Gross(120, "standard")}");
        Console.WriteLine($"express 80 => {Checkout.Gross(80, "express")}");
        return 0;
    }
}
