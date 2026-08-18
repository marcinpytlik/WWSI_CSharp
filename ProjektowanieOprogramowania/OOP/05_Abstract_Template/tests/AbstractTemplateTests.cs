using Oop.AbstractTemplate;
using Xunit;

public class AbstractTemplateTests
{
    [Fact]
    public void Template_Method_Runs_Pipeline()
    {
        var imp = new CsvImporter();
        Assert.Equal("a;b", imp.Run());
    }
}
