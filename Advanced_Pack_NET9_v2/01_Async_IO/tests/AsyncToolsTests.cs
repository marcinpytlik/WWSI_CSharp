
using Advanced.AsyncIO;
using Xunit;

public class AsyncToolsTests
{
    [Fact]
    public async Task Retry_Succeeds_On_Second_Attempt()
    {
        int calls = 0;
        var result = await AsyncTools.RetryAsync(async () =>
        {
            calls++;
            if (calls < 2) throw new InvalidOperationException();
            return 42;
        });
        Assert.Equal(2, calls);
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task WithTimeout_Throws_On_Timeout()
    {
        var t = Task.Delay(200).ContinueWith(_ => 1);
        await Assert.ThrowsAsync<TimeoutException>(() => t.WithTimeout(TimeSpan.FromMilliseconds(50)));
    }
}
