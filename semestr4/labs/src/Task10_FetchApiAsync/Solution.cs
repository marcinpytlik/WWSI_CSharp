namespace Task10_FetchApiAsync;

public static class ApiClient
{
    public static async Task<string> FetchStringAsync(HttpClient http, string url, CancellationToken ct = default)
    {
        using var resp = await http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync(ct);
    }
}
