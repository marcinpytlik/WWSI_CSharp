using GoF.Memento;
using Xunit;

public class MementoTests
{
    [Fact]
    public void Save_And_Restore()
    {
        var e = new Editor();
        e.Type("a"); var snap = e.Save();
        e.Type("b"); e.Restore(snap);
        Assert.Equal("a", e.Text);
    }
}
