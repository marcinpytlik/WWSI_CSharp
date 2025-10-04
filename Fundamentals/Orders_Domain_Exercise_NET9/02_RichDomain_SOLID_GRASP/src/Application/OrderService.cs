
using Orders.Rich.Domain;
using Orders.Rich.Domain.Primitives;

namespace Orders.Rich.Application;

public sealed class OrderService
{
    private readonly IClock _clock;
    private readonly IEmailSender _email;
    private readonly IPricingStrategy _pricing;
    private readonly IVatPolicy _vat;

    public OrderService(IClock clock, IEmailSender email, IPricingStrategy pricing, IVatPolicy vat)
        => (_clock, _email, _pricing, _vat) = (clock, email, pricing, vat);

    public Order Create(string email) => Order.Create(Email.Create(email), _clock.UtcNow);
    public void AddLine(Order o, string sku, int qty, decimal unitPrice) => o.AddLine(sku, qty, new Money(unitPrice, "PLN"));
    public void Pay(Order o) => o.Pay(_clock, _email, _pricing, _vat);
}
