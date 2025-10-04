
using Orders.Rich.Domain.Primitives;

namespace Orders.Rich.Domain;

public interface IClock { DateTime UtcNow { get; } }
public interface IEmailSender { Task SendAsync(string to, string subject, string body); }

public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }
public sealed class NullEmailSender : IEmailSender { public Task SendAsync(string to, string subject, string body) => Task.CompletedTask; }
