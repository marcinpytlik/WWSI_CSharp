using Xunit;

namespace Task30_JsonRoundtripTestTarget.Tests;

public sealed class JsonRoundtripTests
{
    [Fact]
    public void Roundtrip_ReturnsEqualObject()
    {
        var u = new Task30_JsonRoundtripTestTarget.User(1, "sqlmaniak");
        var back = Task30_JsonRoundtripTestTarget.JsonRoundtrip.Roundtrip(u);
        Assert.Equal(u, back);
    }
}
