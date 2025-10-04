
using Xunit;
using Orders.Anemic.Application;

public class OrderServiceStatusTests
{
    private sealed class DummyMail : IEmailSender { public Task SendAsync(string to, string subject, string body) => Task.CompletedTask; }

    [Fact]
    public async Task Pay_Transitions_To_Paid()
    {
        var svc = new OrderService(new SystemClock(), new DummyMail());
        var o = svc.Create("a@b");
        svc.AddLine(o, "SKU", 1, 100m);
        await svc.Pay(o);
        Assert.Equal(Orders.Anemic.Domain.OrderStatus.Paid, o.Status);
    }
}
