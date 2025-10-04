using GoF.Facade;
using Xunit;

public class FacadeTests
{
    [Fact]
    public async Task Simple_Pay_Flow()
    {
        var f = new PaymentFacade();
        Assert.True(await f.PayAsync(10));
    }
}
