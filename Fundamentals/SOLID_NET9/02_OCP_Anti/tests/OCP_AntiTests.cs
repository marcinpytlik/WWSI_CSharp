using Solid.OCP.Anti;
using Xunit;

public class OCP_Anti_Tests
{
    [Fact]
    public void Switch_Grows_With_New_Types()
    {
        var p = new Pricing();
        Assert.Equal(90m, p.Calc("Vip", 100m));
    }
}
