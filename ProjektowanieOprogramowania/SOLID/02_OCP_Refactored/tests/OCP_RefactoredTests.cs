using Solid.OCP.Ref;
using Xunit;

public class OCP_Ref_Tests
{
    [Fact]
    public void New_Variant_Does_Not_Change_Existing_Code()
    {
        var p = PricingFactory.Create("Vip");
        Assert.Equal(90m, p.Calc(100m));
    }
}
