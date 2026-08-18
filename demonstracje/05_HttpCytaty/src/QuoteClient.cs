using System.Net.Http.Json;

namespace Demo05;

public sealed record QuoteDto(string Text, string Author);

public sealed class QuoteClient
{
    private readonly HttpClient _http;

    public QuoteClient(HttpClient http) => _http = http;

    public async Task<QuoteDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var dto = await _http.GetFromJsonAsync<QuoteDto>("/quote", cancellationToken);
        return dto ?? throw new InvalidOperationException("Empty quote payload.");
    }
}

public static class Program
{
    public static async Task<int> Main()
    {
        using var http = new HttpClient { BaseAddress = new Uri("https://example.invalid") };
        Console.WriteLine("Demo uses QuoteClient + injected HttpClient. See tests for a fake handler.");
        await Task.CompletedTask;
        return 0;
    }
}
