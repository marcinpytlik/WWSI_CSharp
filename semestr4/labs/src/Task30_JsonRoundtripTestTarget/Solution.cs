using System.Text.Json;

namespace Task30_JsonRoundtripTestTarget;

public sealed record User(int Id, string Username);

public static class JsonRoundtrip
{
    public static User Roundtrip(User user)
    {
        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var json = JsonSerializer.Serialize(user, opts);
        return JsonSerializer.Deserialize<User>(json, opts)
               ?? throw new InvalidOperationException("Roundtrip failed.");
    }
}
