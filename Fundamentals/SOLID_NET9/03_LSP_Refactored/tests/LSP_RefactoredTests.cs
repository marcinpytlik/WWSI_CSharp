using Solid.LSP.Ref;
using Xunit;

public class LSP_Ref_Tests
{
    [Fact]
    public void Independent_Shapes_Satisfy_Their_Contracts()
    {
        IShape r = new Rectangle { Width = 2, Height = 3 };
        IShape s = new Square { A = 3 };
        Assert.Equal(6, r.Area());
        Assert.Equal(9, s.Area());
    }
}
