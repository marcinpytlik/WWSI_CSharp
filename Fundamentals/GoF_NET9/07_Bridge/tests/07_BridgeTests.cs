using GoF.Bridge;
using Xunit;

public class BridgeTests
{
    [Fact]
    public void Circle_Uses_Renderer()
    {
        var c = new Circle(new VectorRenderer(),1,2,3);
        Assert.StartsWith("vec:", c.Draw());
    }
}
