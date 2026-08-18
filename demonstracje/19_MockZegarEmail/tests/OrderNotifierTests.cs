using Demo19;
using Moq;
using Xunit;

namespace Demo19.Tests;

public class OrderNotifierTests
{
    [Fact]
    public async Task NotifyPaid_SendsEmail_WithClockDate()
    {
        var clock = new Mock<IClock>();
        clock.SetupGet(c => c.UtcNow).Returns(new DateTime(2026, 8, 18, 12, 0, 0, DateTimeKind.Utc));
        var email = new Mock<IEmailSender>();
        var svc = new OrderNotifier(clock.Object, email.Object);

        await svc.NotifyPaidAsync("ada@example.com", 19.90m);

        email.Verify(e => e.SendAsync(
            "ada@example.com",
            "Order paid 2026-08-18",
            "amount=19.90",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvalidEmail_DoesNotSend()
    {
        var email = new Mock<IEmailSender>();
        var svc = new OrderNotifier(Mock.Of<IClock>(), email.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => svc.NotifyPaidAsync(" ", 10m));
        email.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
