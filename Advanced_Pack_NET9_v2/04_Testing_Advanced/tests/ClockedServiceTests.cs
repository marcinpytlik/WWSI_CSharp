
using Advanced.TestingAdvanced;
using Xunit;

public class ClockedServiceTests
{
    private sealed class FixedClock : IClock { public DateTime UtcNow { get; init; } }

    [Fact]
    public void Deterministic_Stamp()
    {
        var svc = new ClockedService(new FixedClock{ UtcNow = new DateTime(2025,1,1,0,0,0,DateTimeKind.Utc)});
        var s = svc.Stamp("hi");
        Assert.StartsWith("2025-01-01T00:00:00.0000000Z|", s);
    }
}
