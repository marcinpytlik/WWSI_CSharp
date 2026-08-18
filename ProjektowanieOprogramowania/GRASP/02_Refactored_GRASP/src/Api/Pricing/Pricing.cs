namespace Grasp.Refactored.Pricing;

public interface IPricingStrategy
{
    decimal Apply(decimal baseTotal);
}

public sealed class VipPricing : IPricingStrategy
{
    public decimal Apply(decimal baseTotal) => baseTotal * 0.9m;
}

public sealed class RegularPricing : IPricingStrategy
{
    public decimal Apply(decimal baseTotal) => baseTotal;
}

public interface IVatPolicy
{
    decimal Apply(decimal net);
}

public sealed class Vat23 : IVatPolicy
{
    public decimal Apply(decimal net) => Math.Round(net * 1.23m, 2);
}

public interface IPricingStrategyFactory
{
    IPricingStrategy For(bool isVip);
}

public sealed class PricingStrategyFactory : IPricingStrategyFactory
{
    public IPricingStrategy For(bool isVip) => isVip ? new VipPricing() : new RegularPricing();
}
