
using Orders.Rich.Domain.Primitives;

namespace Orders.Rich.Domain;

public interface IPricingStrategy { Money Apply(Money baseTotal); }
public sealed class VipPricing : IPricingStrategy { public Money Apply(Money baseTotal) => baseTotal * 0.9m; }
public sealed class RegularPricing : IPricingStrategy { public Money Apply(Money baseTotal) => baseTotal; }

public interface IVatPolicy { Money Apply(Money net); }
public sealed class Vat23 : IVatPolicy { public Money Apply(Money net) => new Money(net.Amount * 1.23m, net.Currency); }
