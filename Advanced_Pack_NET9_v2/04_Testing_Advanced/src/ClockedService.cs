
namespace Advanced.TestingAdvanced;

public interface IClock { DateTime UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }

public sealed class ClockedService(IClock clock)
{
    public string Stamp(string msg) => $"{clock.UtcNow:O}|{msg}";
}
