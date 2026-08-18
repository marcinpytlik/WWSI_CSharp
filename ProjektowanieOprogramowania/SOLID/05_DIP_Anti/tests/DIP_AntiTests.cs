using Solid.DIP.Anti;
using Xunit;

public class DIP_Anti_Tests
{
    [Fact]
    public void Hard_To_Test_Time()
    {
        var svc = new TokenService();
        var exp = svc.Expiry();
        Assert.True((exp - DateTime.UtcNow).TotalMinutes <= 30.0);
    }
}
