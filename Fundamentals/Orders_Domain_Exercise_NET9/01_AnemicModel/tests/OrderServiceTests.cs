
using Xunit;
using Orders.Anemic.Domain;
using Orders.Anemic.Application;

public class OrderServiceTests
{
    [Fact]
    public void Create_AddLine_Calc_Works()
    {
        var svc = new OrderService(new SystemClock(), new NullEmailSender());
        var o = svc.Create("a@b");
        svc.AddLine(o, "SKU", 2, 50m);
        var total = svc.CalculateTotal(o, isVip: true);
        Assert.Equal(110.70m, total); // (2*50)=100 -> VIP 10% => 90 -> VAT 23% => 110.70
    }
}
