namespace Solid.OCP.Ref;

public interface IPricing { decimal Calc(decimal basePrice); }
public class NormalPricing : IPricing { public decimal Calc(decimal basePrice) => basePrice; }
public class VipPricing : IPricing { public decimal Calc(decimal basePrice) => basePrice * 0.9m; }

public static class PricingFactory
{
    public static IPricing Create(string type) => type switch
    {
        "Normal" => new NormalPricing(),
        "Vip" => new VipPricing(),
        _ => new NormalPricing()
    };
}
