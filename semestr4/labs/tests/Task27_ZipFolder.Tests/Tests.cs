using Xunit;
using System.IO.Compression;

namespace Task27_ZipFolder.Tests;

public sealed class FolderZipperTests
{
    [Fact]
    public void Zip_CreatesZipWithFiles()
    {
        var dir = Path.Combine(Path.GetTempPath(), "zip_" + Guid.NewGuid());
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "a.txt"), "A");
        File.WriteAllText(Path.Combine(dir, "b.txt"), "B");

        var zip = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".zip");
        Task27_ZipFolder.FolderZipper.Zip(dir, zip);

        Assert.True(File.Exists(zip));

        using var archive = ZipFile.OpenRead(zip);
        Assert.NotNull(archive.GetEntry("a.txt"));
        Assert.NotNull(archive.GetEntry("b.txt"));

        Directory.Delete(dir, true);
        File.Delete(zip);
    }
}
