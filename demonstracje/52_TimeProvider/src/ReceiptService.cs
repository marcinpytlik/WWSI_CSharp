using System.Globalization;

namespace Demo52;

public sealed class ReceiptService
{
    private readonly TimeProvider _clock;
    public ReceiptService(TimeProvider clock) => _clock = clock;

    public string PaidSubject(decimal amount)
        => string.Create(
            CultureInfo.InvariantCulture,
            $"Paid {amount:0.00} at {_clock.GetUtcNow().UtcDateTime:yyyy-MM-dd}");
}

public static class Program
{
    public static int Main()
    {
        Console.WriteLine(new ReceiptService(TimeProvider.System).PaidSubject(10));
        return 0;
    }
}
