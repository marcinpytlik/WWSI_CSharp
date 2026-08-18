using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; // (not used, but typical place for infra)
using Microsoft.Extensions.Options;

// ========== Ports (Indirection, Protected Variations) ==========
public interface IClock { DateTime UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }

public interface IEmailSender { Task SendAsync(string to, string subject, string body); }
public sealed class ConsoleEmailSender : IEmailSender
{
    public Task SendAsync(string to, string subject, string body) { Console.WriteLine($"MAIL -> {to}: {subject}"); return Task.CompletedTask; }
}

// ========== Domain (Information Expert) ==========
public sealed record OrderLine(string Sku, int Qty, decimal UnitPrice)
{
    public decimal LineTotal() => Qty * UnitPrice;
}

public sealed class Order
{
    private readonly List<OrderLine> _lines = new();
    public Guid Id { get; } = Guid.NewGuid();
    public string Email { get; }
    public DateTime CreatedUtc { get; }
    public Order(string email, DateTime createdUtc) { Email = email; CreatedUtc = createdUtc; }

    public void AddLine(OrderLine line) => _lines.Add(line);
    public decimal BaseTotal() => _lines.Sum(l => l.LineTotal());
}

// ========== Polymorphism (Strategy) ==========
public interface IPricingStrategy { decimal Apply(decimal baseTotal); }
public sealed class VipPricing : IPricingStrategy { public decimal Apply(decimal baseTotal) => baseTotal * 0.9m; }
public sealed class RegularPricing : IPricingStrategy { public decimal Apply(decimal baseTotal) => baseTotal; }

// VAT policy as separate strategy if desired
public interface IVatPolicy { decimal Apply(decimal net); }
public sealed class Vat23 : IVatPolicy { public decimal Apply(decimal net) => Math.Round(net * 1.23m, 2); }

// ========== Repository (Pure Fabrication) ==========
public interface IOrderRepository { Task SaveAsync(Order order, decimal finalAmount); }
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<(Guid Id, string Email, decimal Final, DateTime CreatedUtc)> _db = new();
    public Task SaveAsync(Order order, decimal finalAmount)
    {
        _db.Add((order.Id, order.Email, finalAmount, order.CreatedUtc));
        return Task.CompletedTask;
    }
}

// ========== Application Service (High Cohesion) ==========
public sealed class OrderService
{
    private readonly IClock _clock;
    private readonly IEmailSender _email;
    private readonly IOrderRepository _repo;

    public OrderService(IClock clock, IEmailSender email, IOrderRepository repo)
        => (_clock, _email, _repo) = (clock, email, repo);

    public async Task<(Guid id, decimal final)> CreateAsync(CreateOrderDto dto, IPricingStrategy pricing, IVatPolicy vat)
    {
        Validate(dto);

        var order = new Order(dto.Email, _clock.UtcNow);
        foreach (var l in dto.Lines)
            order.AddLine(new OrderLine(l.Sku, l.Qty, l.UnitPrice));

        var baseTotal = order.BaseTotal();
        var priced = pricing.Apply(baseTotal);
        var final = vat.Apply(priced);

        await _repo.SaveAsync(order, final);
        await _email.SendAsync(dto.Email, "Order confirmation", $"Amount: {final}");

        return (order.Id, final);
    }

    private static void Validate(CreateOrderDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
            throw new ArgumentException("Invalid email");
        if (dto.Lines is null || dto.Lines.Length == 0)
            throw new ArgumentException("At least one line");
        if (dto.Lines.Any(l => l.Qty <= 0 || l.UnitPrice <= 0))
            throw new ArgumentException("Invalid line");
    }
}

// ========== Controller (GRASP Controller) ==========
public sealed class OrderController
{
    private readonly OrderService _svc;
    private readonly IServiceProvider _sp; // indirection for strategies

    public OrderController(OrderService svc, IServiceProvider sp)
        => (_svc, _sp) = (svc, sp);

    public async Task<IResult> Create(CreateOrderDto dto)
    {
        try
        {
            // choose strategy (Protected Variations) — e.g., feature flag / customer flag
            IPricingStrategy pricing = dto.IsVip ? _sp.GetRequiredService<VipPricing>() : _sp.GetRequiredService<RegularPricing>();
            IVatPolicy vat = _sp.GetRequiredService<Vat23>();

            var (id, final) = await _svc.CreateAsync(dto, pricing, vat);
            return Results.Created($"/api/v1/orders/{id}", new { id, final });
        }
        catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
    }
}

// ========== DTOs (Indirection boundary) ==========
public sealed record CreateOrderDto(string Email, bool IsVip, LineDto[] Lines);
public sealed record LineDto(string Sku, int Qty, decimal UnitPrice);

// ========== Composition Root ==========
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderController>();
builder.Services.AddSingleton<VipPricing>(); builder.Services.AddSingleton<RegularPricing>(); builder.Services.AddSingleton<Vat23>();

var app = builder.Build();
app.UseSwagger(); app.UseSwaggerUI();

app.MapPost("/api/v1/orders",
    async ([FromBody] CreateOrderDto dto, OrderController c) => await c.Create(dto))
   .WithName("CreateOrder_Refactored")
   .WithOpenApi();

app.MapGet("/api/v1/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program {}
