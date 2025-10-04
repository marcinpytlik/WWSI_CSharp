namespace Solid.DIP.Ref;

public interface IClock { DateTime UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }

public class TokenService
{
    private readonly IClock _clock;
    public TokenService(IClock clock) => _clock = clock;
    public DateTime Expiry() => _clock.UtcNow.AddMinutes(30);
}
