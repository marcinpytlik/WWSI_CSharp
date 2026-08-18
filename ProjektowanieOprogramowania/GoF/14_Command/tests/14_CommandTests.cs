using GoF.Command;
using Xunit;

public class CommandTests
{
    [Fact]
    public void Add_And_Undo()
    {
        var list = new List<int>();
        var cmd = new AddItem(list, 7);
        cmd.Execute(); Assert.Contains(7, list);
        cmd.Undo(); Assert.DoesNotContain(7, list);
    }
}
