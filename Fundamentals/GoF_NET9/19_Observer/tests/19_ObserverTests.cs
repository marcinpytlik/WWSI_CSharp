using GoF.Observer;
using Xunit;

public class ObserverTests
{
    [Fact]
    public void Notifies_Subscribers()
    {
        var t = new Topic();
        string? seen = null;
        t.OnMsg += s => seen = s;
        t.Publish("hi");
        Assert.Equal("hi", seen);
    }
}
