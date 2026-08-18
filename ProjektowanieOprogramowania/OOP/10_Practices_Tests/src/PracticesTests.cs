namespace Oop.Practices;

public interface IClock { DateTime UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }

public class TokenService(IClock clock)
{
    public DateTime Expiry() => clock.UtcNow.AddMinutes(30);
}
