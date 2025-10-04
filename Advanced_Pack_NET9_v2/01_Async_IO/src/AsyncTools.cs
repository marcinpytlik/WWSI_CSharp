
using System.Runtime.CompilerServices;

namespace Advanced.AsyncIO;

public static class AsyncTools
{
    // Timeout via Task.WhenAny
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, CancellationToken ct = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var delay = Task.Delay(timeout, cts.Token);
        var done = await Task.WhenAny(task, delay).ConfigureAwait(false);
        if (done == delay) throw new TimeoutException();
        cts.Cancel(); // cancel delay
        return await task.ConfigureAwait(false);
    }

    // Simple retry with linear backoff
    public static async Task<T> RetryAsync<T>(Func<Task<T>> action, int attempts = 3, TimeSpan? backoff = null)
    {
        backoff ??= TimeSpan.FromMilliseconds(100);
        Exception? last = null;
        for (int i = 0; i < attempts; i++)
        {
            try { return await action().ConfigureAwait(false); }
            catch (Exception ex) { last = ex; if (i < attempts - 1) await Task.Delay(backoff.Value * (i + 1)).ConfigureAwait(false); }
        }
        throw last ?? new Exception("Retry failed");
    }

    // IAsyncEnumerable demo
    public static async IAsyncEnumerable<int> CountAsync(int n, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; i < n; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return i;
        }
    }
}
