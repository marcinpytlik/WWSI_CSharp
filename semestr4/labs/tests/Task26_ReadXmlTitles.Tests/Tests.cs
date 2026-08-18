using Xunit;

namespace Task26_ReadXmlTitles.Tests;

public sealed class XmlTitleReaderTests
{
    [Fact]
    public void ReadTitles_ReturnsAllTitles()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xml");
        File.WriteAllText(path, "<root><item><title>A</title></item><item><title>B</title></item></root>");

        var titles = Task26_ReadXmlTitles.XmlTitleReader.ReadTitles(path);

        Assert.Equal(new[] { "A", "B" }, titles);
        File.Delete(path);
    }
}
