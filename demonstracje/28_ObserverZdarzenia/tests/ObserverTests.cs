using Demo28;
using Xunit;

namespace Demo28.Tests;

public class ObserverTests
{
    [Fact]
    public void Place_Notifies_AllListeners()
    {
        var desk = new OrderDesk();
        var mail = new EmailListener();
        var other = new EmailListener();
        desk.Placed += mail.OnPlaced;
        desk.Placed += other.OnPlaced;
        desk.Place("SKU-9", 3);
        Assert.Equal(["SKU-9 x3"], mail.Inbox);
        Assert.Equal(mail.Inbox, other.Inbox);
    }

    [Fact]
    public void Unsubscribed_DoesNotReceive()
    {
        var desk = new OrderDesk();
        var mail = new EmailListener();
        desk.Placed += mail.OnPlaced;
        desk.Placed -= mail.OnPlaced;
        desk.Place("SKU-1", 1);
        Assert.Empty(mail.Inbox);
    }
}
