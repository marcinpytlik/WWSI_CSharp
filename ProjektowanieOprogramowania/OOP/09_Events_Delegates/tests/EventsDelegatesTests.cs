using Oop.Events;
using Xunit;

public class EventsDelegatesTests
{
    [Fact]
    public void Raises_Event()
    {
        var a = new Alarm();
        string? seen = null;
        a.OnAlert += m => seen = m;
        a.Trigger("Disk full");
        Assert.Equal("Disk full", seen);
    }
}
