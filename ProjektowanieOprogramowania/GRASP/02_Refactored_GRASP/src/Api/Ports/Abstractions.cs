using Grasp.Refactored.Domain;

namespace Grasp.Refactored.Ports;

public interface IClock
{
    DateTime UtcNow { get; }
}

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body);
}

public interface IOrderRepository
{
    Task SaveAsync(Order order, decimal finalAmount);
}
