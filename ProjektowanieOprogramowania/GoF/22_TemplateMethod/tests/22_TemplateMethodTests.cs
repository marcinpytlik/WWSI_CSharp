using GoF.Template;
using Xunit;

public class TemplateMethodTests
{
    [Fact]
    public void Runs_Pipeline()
    {
        var imp = new CsvImporter();
        Assert.Equal("a;b", imp.Run());
    }
}
