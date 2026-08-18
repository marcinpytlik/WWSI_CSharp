using System.Text.Json;
using Demo49;
using Xunit;

namespace Demo49.Tests;

public class BuildPropsTests
{
    [Fact]
    public void NestedDirectoryBuildProps_DefinesDEMO49()
        => Assert.True(BuildFlags.FromNestedBuildProps);

    [Fact]
    public void AssemblyTitle_ComesFromNestedProps()
        => Assert.Equal("WWSI Demo49", BuildFlags.AssemblyTitle());

    [Fact]
    public void GlobalJson_PinsNet10()
    {
        var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "global.json"));
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        var version = doc.RootElement.GetProperty("sdk").GetProperty("version").GetString();
        Assert.StartsWith("10.", version);
        Assert.Equal("latestFeature", doc.RootElement.GetProperty("sdk").GetProperty("rollForward").GetString());
    }
}
