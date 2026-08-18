using Grasp.Refactored.Domain;
using Grasp.Refactored.Ports;

namespace Grasp.Refactored.Infrastructure;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public sealed class ConsoleEmailSender : IEmailSender
{
    public Task SendAsync(string to, string subject, string body)
    {
        Console.WriteLine($"MAIL -> {to}: {subject}");
        return Task.CompletedTask;
    }
}

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<(Guid Id, string Email, decimal Final, DateTime CreatedUtc)> _db = new();

    public IReadOnlyList<(Guid Id, string Email, decimal Final, DateTime CreatedUtc)> Items => _db;

    public Task SaveAsync(Order order, decimal finalAmount)
    {
        _db.Add((order.Id, order.Email, finalAmount, order.CreatedUtc));
        return Task.CompletedTask;
    }
}
