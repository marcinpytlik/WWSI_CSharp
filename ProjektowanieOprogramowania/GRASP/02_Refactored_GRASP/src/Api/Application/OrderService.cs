using Grasp.Refactored.Contracts;
using Grasp.Refactored.Domain;
using Grasp.Refactored.Ports;
using Grasp.Refactored.Pricing;

namespace Grasp.Refactored.Application;

public sealed class OrderService
{
    private readonly IClock _clock;
    private readonly IEmailSender _email;
    private readonly IOrderRepository _repo;
    private readonly IPricingStrategyFactory _pricingFactory;
    private readonly IVatPolicy _vat;

    public OrderService(
        IClock clock,
        IEmailSender email,
        IOrderRepository repo,
        IPricingStrategyFactory pricingFactory,
        IVatPolicy vat)
    {
        _clock = clock;
        _email = email;
        _repo = repo;
        _pricingFactory = pricingFactory;
        _vat = vat;
    }

    public async Task<(Guid Id, decimal Final)> CreateAsync(CreateOrderDto dto)
    {
        Validate(dto);

        var order = new Order(dto.Email, _clock.UtcNow);
        foreach (var line in dto.Lines)
            order.AddLine(new OrderLine(line.Sku, line.Qty, line.UnitPrice));

        var priced = _pricingFactory.For(dto.IsVip).Apply(order.BaseTotal());
        var final = _vat.Apply(priced);

        await _repo.SaveAsync(order, final);
        await _email.SendAsync(dto.Email, "Order confirmation", $"Amount: {final}");

        return (order.Id, final);
    }

    private static void Validate(CreateOrderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
            throw new ArgumentException("Invalid email");
        if (dto.Lines is null || dto.Lines.Length == 0)
            throw new ArgumentException("At least one line");
        if (dto.Lines.Any(l => l.Qty <= 0 || l.UnitPrice <= 0))
            throw new ArgumentException("Invalid line");
    }
}
