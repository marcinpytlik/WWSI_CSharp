using Demo64;
using Xunit;

namespace Demo64.Tests;

public class TemplateMethodTests
{
    [Fact]
    public void Csv_AndJson_ShareValidation()
    {
        var csv = new CsvImporter().Import("sku-1,2\nsku-2,0\nsku-3,1");
        var json = new JsonImporter().Import("""[{"sku":"sku-1","qty":2},{"sku":"sku-2","qty":0}]""");
        Assert.Equal(new[] { "SKU-1", "SKU-3" }, csv.Select(r => r.Sku));
        Assert.Equal(new[] { "SKU-1" }, json.Select(r => r.Sku));
    }
}
