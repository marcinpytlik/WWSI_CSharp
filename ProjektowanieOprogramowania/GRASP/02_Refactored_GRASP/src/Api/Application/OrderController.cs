using Grasp.Refactored.Contracts;

namespace Grasp.Refactored.Application;

public sealed class OrderController
{
    private readonly OrderService _svc;

    public OrderController(OrderService svc) => _svc = svc;

    public async Task<IResult> Create(CreateOrderDto dto)
    {
        try
        {
            var (id, final) = await _svc.CreateAsync(dto);
            return Results.Created($"/api/v1/orders/{id}", new { id, final });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}
