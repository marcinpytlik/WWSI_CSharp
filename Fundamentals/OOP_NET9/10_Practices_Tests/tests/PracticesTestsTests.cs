using Oop.Practices;
using Xunit;

public class PracticesTests
{
    private sealed class FixedClock : IClock { public DateTime UtcNow { get; init; } }

    [Fact]
    public void Deterministic_Time_Thanks_To_DIP()
    {
        var svc = new TokenService(new FixedClock { UtcNow = new DateTime(2025,1,1,0,0,0,DateTimeKind.Utc) });
        Assert.Equal(new DateTime(2025,1,1,0,30,0,DateTimeKind.Utc), svc.Expiry());
    }
}
