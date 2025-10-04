using Solid.DIP.Ref;
using Xunit;

public class DIP_Ref_Tests
{
    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow { get; init; }
    }

    [Fact]
    public void Deterministic_Time_In_Tests()
    {
        var svc = new TokenService(new FixedClock { UtcNow = new DateTime(2025,1,1,0,0,0,DateTimeKind.Utc) });
        Assert.Equal(new DateTime(2025,1,1,0,30,0,DateTimeKind.Utc), svc.Expiry());
    }
}
