using System.Text.Json;

namespace Task09_WriteUserJson;

public sealed record User(int Id, string Username, string Email);

public static class UserJsonWriter
{
    public static async Task WriteAsync(User user, string path, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(user, new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });
        await File.WriteAllTextAsync(path, json, ct);
    }

    public static async Task<User> ReadAsync(string path, CancellationToken ct = default)
    {
        var json = await File.ReadAllTextAsync(path, ct);
        return JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web))
               ?? throw new InvalidOperationException("Invalid JSON file.");
    }
}
