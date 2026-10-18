var permanent = args.Contains("permanent", StringComparer.OrdinalIgnoreCase);
var attempts = 0;
async Task<string> Unstable(CancellationToken ct)
{
    attempts++;
    await Task.Delay(150, ct);
    if (permanent || attempts < 3) throw new TimeoutException("Symulowany timeout usługi");
    return "OK";
}
for (int i = 1; i <= 4; i++)
{
    try
    {
        var result = await Unstable(CancellationToken.None);
        Console.WriteLine($"Attempt {i}: {result}"); break;
    }
    catch (TimeoutException ex)
    {
        Console.WriteLine($"Attempt {i}: {ex.Message}");
        if (i == 4) Console.WriteLine("Retry exhausted");
        else await Task.Delay(200 * i);
    }
}
Console.WriteLine($"Total attempts={attempts}; retry tylko dla operacji bezpiecznych do ponowienia.");
