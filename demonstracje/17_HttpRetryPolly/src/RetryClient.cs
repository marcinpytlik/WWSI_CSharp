using System.Net.Http.Json;
using Polly;
using Polly.Retry;

namespace Demo17;

public sealed record QuoteDto(string Text);

public static class QuoteRetry
{
    public static ResiliencePipeline<HttpResponseMessage> Create(int maxAttempts = 3)
        => new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(r => (int)r.StatusCode >= 500),
                MaxRetryAttempts = maxAttempts,
                Delay = TimeSpan.Zero
            })
            .Build();
}

public sealed class QuoteClient
{
    private readonly HttpClient _http;
    private readonly ResiliencePipeline<HttpResponseMessage> _retry;

    public QuoteClient(HttpClient http, ResiliencePipeline<HttpResponseMessage>? retry = null)
    {
        _http = http;
        _retry = retry ?? QuoteRetry.Create();
    }

    public async Task<QuoteDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var response = await _retry.ExecuteAsync(
            async ct => await _http.GetAsync("/quote", ct),
            cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<QuoteDto>(cancellationToken)
            ?? throw new InvalidOperationException("Empty quote.");
    }
}

public static class Program
{
    public static async Task<int> Main()
    {
        Console.WriteLine("QuoteClient + Polly retry. See tests (fake handler: 500, 500, 200).");
        await Task.CompletedTask;
        return 0;
    }
}
