using Xunit;
using LabApp;

public class ReadmeExampleTests
{
    [Fact]
    public void Echo_Returns_EMPTY_OnNullOrWhitespace()
        => Assert.Equal("EMPTY", ReadmeExample.Echo("   "));

    [Fact]
    public void Echo_Trims_Input()
        => Assert.Equal("abc", ReadmeExample.Echo("  abc  "));
}
