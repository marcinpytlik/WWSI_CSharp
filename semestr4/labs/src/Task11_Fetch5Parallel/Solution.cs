namespace Task11_Fetch5Parallel;

public static class ParallelFetcher
{
    public static async Task<IReadOnlyList<string>> FetchFirst5Async(HttpClient http, IEnumerable<string> urls, CancellationToken ct = default)
    {
        var list = urls.Take(5).ToList();
        var tasks = list.Select(async u =>
        {
            using var resp = await http.GetAsync(u, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(ct);
        });

        return await Task.WhenAll(tasks);
    }
}
