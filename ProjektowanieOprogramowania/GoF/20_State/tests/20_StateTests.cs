using GoF.State;
using Xunit;

public class StateTests
{
    [Fact]
    public void Advances_States()
    {
        var d = new Doc();
        d.Advance(); Assert.Equal("Published", d.State.Name);
        d.Advance(); Assert.Equal("Archived", d.State.Name);
    }
}
