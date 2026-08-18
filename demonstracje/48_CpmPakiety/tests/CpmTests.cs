using System.Reflection;
using System.Xml.Linq;
using Demo48.LibA;
using Demo48.LibB;
using Xunit;

namespace Demo48.Tests;

public class CpmTests
{
    [Fact]
    public void BothLibraries_UseSameFluentValidationVersion()
    {
        var a = typeof(EmailValidator).Assembly.GetReferencedAssemblies()
            .Single(n => n.Name == "FluentValidation").Version;
        var b = typeof(SkuValidator).Assembly.GetReferencedAssemblies()
            .Single(n => n.Name == "FluentValidation").Version;
        Assert.Equal(a, b);
        Assert.NotNull(a);
    }

    [Fact]
    public void PackageReferences_HaveNoLocalVersion()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        foreach (var name in new[] { "src/LibA/Demo48_LibA.csproj", "src/LibB/Demo48_LibB.csproj" })
        {
            var doc = XDocument.Load(Path.Combine(root, name));
            var refs = doc.Descendants("PackageReference")
                .Where(e => (string?)e.Attribute("Include") == "FluentValidation");
            Assert.Contains(refs, _ => true);
            Assert.All(refs, e => Assert.Null(e.Attribute("Version")));
        }
    }

    [Fact]
    public void Validators_Work()
    {
        Assert.True(new EmailValidator().Validate(new EmailRequest { Email = "ada@wwsi.edu.pl" }).IsValid);
        Assert.False(new SkuValidator().Validate(new SkuRequest { Sku = "x" }).IsValid);
    }
}
