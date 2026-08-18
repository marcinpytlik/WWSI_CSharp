using Microsoft.Extensions.Logging;

namespace Demo61;

public static class SqlRetry
{
    public static async Task WaitAsync(Func<Task> action, ILogger logger, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (
                attempt < 20
                && !cancellationToken.IsCancellationRequested
                && ex is not InvalidOperationException)
            {
                logger.LogWarning(ex, "SQL niegotowe (próba {Attempt}/20).", attempt);
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }
    }
}
