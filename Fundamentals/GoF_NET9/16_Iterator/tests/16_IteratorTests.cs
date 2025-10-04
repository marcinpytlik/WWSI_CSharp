using GoF.Iterator;
using Xunit;
using System.Linq;

public class IteratorTests
{
    [Fact]
    public void Enumerates_Range()
    {
        var r = new Range();
        Assert.Equal(new[]{0,1,2}, r.ToArray());
    }
}
