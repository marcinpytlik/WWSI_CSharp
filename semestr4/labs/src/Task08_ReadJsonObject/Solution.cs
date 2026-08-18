using System.Text.Json;

namespace Task08_ReadJsonObject;

public sealed record Person(int Id, string Name, int Age);

public static class JsonReader
{
    public static Person ReadPerson(string json)
    {
        var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        return JsonSerializer.Deserialize<Person>(json, opts)
               ?? throw new InvalidOperationException("Invalid JSON.");
    }
}
