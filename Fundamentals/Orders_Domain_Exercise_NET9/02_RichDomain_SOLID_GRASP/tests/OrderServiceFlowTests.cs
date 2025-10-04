
using Xunit;
using Orders.Rich.Domain;
using Orders.Rich.Application;

public class OrderServiceFlowTests
{
    private sealed class FixedClock : IClock { public DateTime UtcNow { get; init; } }
    private sealed class SpyEmail : IEmailSender { public List<string> Sent = new(); public Task SendAsync(string to, string subject, string body){ Sent.Add($"{to}|{subject}|{body}"); return Task.CompletedTask; } }

    [Fact]
    public void Create_Add_Pay_Works_With_Strategies()
    {
        var email = new SpyEmail();
        var svc = new OrderService(new FixedClock{ UtcNow = new DateTime(2025,1,1,0,0,0,DateTimeKind.Utc) }, email, new VipPricing(), new Vat23());
        var o = svc.Create("a@b");
        svc.AddLine(o, "SKU", 2, 50m);
        svc.Pay(o);
        Assert.Equal(OrderStatus.Paid, o.Status);
        Assert.Contains("Order paid", email.Sent[0]);
    }
}
