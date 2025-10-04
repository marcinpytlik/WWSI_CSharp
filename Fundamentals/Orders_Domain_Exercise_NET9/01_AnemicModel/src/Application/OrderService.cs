
namespace Orders.Anemic.Application;

using Orders.Anemic.Domain;
using System.Text.RegularExpressions;

public interface IClock { DateTime UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }

public interface IEmailSender { Task SendAsync(string to, string subject, string body); }
public sealed class NullEmailSender : IEmailSender { public Task SendAsync(string to, string subject, string body) => Task.CompletedTask; }

public class OrderService
{
    private readonly IClock _clock;
    private readonly IEmailSender _email;

    public OrderService(IClock clock, IEmailSender email) { _clock = clock; _email = email; }

    public Order Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, ".+@.+")) throw new ArgumentException("Invalid email");
        return new Order { Id = Guid.NewGuid(), CustomerEmail = email, Lines = new(), Status = OrderStatus.New };
    }

    public void AddLine(Order order, string productId, int qty, decimal unitPrice)
    {
        if (qty <= 0 || unitPrice <= 0) throw new ArgumentException("Invalid line");
        order.Lines.Add(new OrderLine { ProductId = productId, Qty = qty, UnitPrice = unitPrice });
    }

    public decimal CalculateTotal(Order order, bool isVip)
    {
        var net = order.Lines.Sum(l => l.Qty * l.UnitPrice);
        if (isVip) net *= 0.9m;
        var gross = Math.Round(net * 1.23m, 2);
        return gross;
    }

    public async Task Pay(Order order)
    {
        if (order.Status != OrderStatus.New) throw new InvalidOperationException("Invalid status");
        order.Status = OrderStatus.Paid;
        await _email.SendAsync(order.CustomerEmail, "Order paid", "Thanks!");
    }
}
