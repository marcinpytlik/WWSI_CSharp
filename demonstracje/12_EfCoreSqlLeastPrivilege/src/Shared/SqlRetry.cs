namespace Demo12;

public static class SqlRetry
{
    public static async Task WaitAsync(Func<Task> action, TextWriter log, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex) when (attempt < 20 && !cancellationToken.IsCancellationRequested)
            {
                log.WriteLine($"{ex.GetType().Name}: SQL niegotowe (próba {attempt}/20). Ponawiam za 3 s…");
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }
    }
}
