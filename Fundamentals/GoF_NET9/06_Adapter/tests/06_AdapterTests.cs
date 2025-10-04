using GoF.Adapter;
using Xunit;

public class AdapterTests
{
    [Fact]
    public void Adapts_To_New_Interface()
    {
        INewPay p = new PayAdapter();
        Assert.StartsWith("OLD:", p.Pay(12.34m));
    }
}
