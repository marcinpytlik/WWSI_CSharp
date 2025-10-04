using GoF.FactoryMethod;
using Xunit;

public class FactoryMethodTests
{
    [Fact] public void PdfReport_Uses_PdfExporter()
        => Assert.Equal("PDF", new PdfReport().Export());

    [Fact] public void HtmlReport_Uses_HtmlExporter()
        => Assert.Equal("HTML", new HtmlReport().Export());
}
