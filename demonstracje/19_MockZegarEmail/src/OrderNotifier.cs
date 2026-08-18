namespace Demo19;

public interface IClock
{
    DateTime UtcNow { get; }
}

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public sealed class OrderNotifier
{
    private readonly IClock _clock;
    private readonly IEmailSender _email;

    public OrderNotifier(IClock clock, IEmailSender email)
    {
        _clock = clock;
        _email = email;
    }

    public async Task NotifyPaidAsync(string email, decimal amount, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        var subject = $"Order paid {_clock.UtcNow:yyyy-MM-dd}";
        await _email.SendAsync(email, subject, $"amount={amount}", cancellationToken);
    }
}

public static class Program
{
    public static int Main()
    {
        Console.WriteLine("OrderNotifier depends on IClock + IEmailSender. See Moq tests.");
        return 0;
    }
}
