using Solid.ISP.Ref;
using Xunit;

public class ISP_Ref_Tests
{
    [Fact]
    public void Split_Interfaces_Avoid_NotImplemented()
    {
        IReader<int> r = new SimpleReader();
        Assert.Equal(2, r.Get(2));
    }
}
