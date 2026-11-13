using System.Text;
using QueueCacheDemo.Errors;
using Xunit;

public sealed class RetryAttemptParserTests
{
    [Fact]
    public void FirstDeliveryHasZeroAttempts() =>
        Assert.Equal(0, RetryAttemptParser.Parse(null));

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void NumericRabbitHeadersPreserveAttempts(int attempts) =>
        Assert.Equal(attempts, RetryAttemptParser.Parse(attempts));

    [Fact]
    public void EncodedRabbitHeaderPreservesAttempt() =>
        Assert.Equal(1, RetryAttemptParser.Parse(Encoding.UTF8.GetBytes("1")));

    [Fact]
    public void BigEndianIntegerHeaderPreservesAttempt() =>
        Assert.Equal(2, RetryAttemptParser.Parse(new byte[] { 0, 0, 0, 2 }));

    [Fact]
    public void MalformedAttemptFailsClosed() =>
        Assert.Equal(3, RetryAttemptParser.Parse("garbage"));
}
