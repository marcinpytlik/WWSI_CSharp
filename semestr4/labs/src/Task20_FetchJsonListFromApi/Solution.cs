using System.Text.Json;

namespace Task20_FetchJsonListFromApi;

public static class JsonListFetcher
{
    public static async Task<List<T>> FetchListAsync<T>(HttpClient http, string url, CancellationToken ct = default)
    {
        var json = await http.GetStringAsync(url, ct);
        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        return JsonSerializer.Deserialize<List<T>>(json, opts) ?? new List<T>();
    }
}
