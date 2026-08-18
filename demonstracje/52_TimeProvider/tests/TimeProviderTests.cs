using Demo52;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace Demo52.Tests;

public class TimeProviderTests
{
    [Fact]
    public void FakeTimeProvider_FixesDate_WithoutMoq()
    {
        var clock = new FakeTimeProvider(new DateTimeOffset(2026, 8, 18, 12, 0, 0, TimeSpan.Zero));
        var subject = new ReceiptService(clock).PaidSubject(19.9m);
        Assert.Equal("Paid 19.90 at 2026-08-18", subject);
    }
}
