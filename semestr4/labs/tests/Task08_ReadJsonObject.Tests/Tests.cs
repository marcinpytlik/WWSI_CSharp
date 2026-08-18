using Xunit;

namespace Task08_ReadJsonObject.Tests;

public sealed class JsonReaderTests
{
    [Fact]
    public void ReadPerson_Deserializes()
    {
        var json = """{ "id": 1, "name": "Marcin", "age": 52 }""";
        var p = Task08_ReadJsonObject.JsonReader.ReadPerson(json);

        Assert.Equal(1, p.Id);
        Assert.Equal("Marcin", p.Name);
        Assert.Equal(52, p.Age);
    }
}
