using Xunit;

namespace Task18_CopyFileStreamReaderWriter.Tests;

public sealed class FileCopierTests
{
    [Fact]
    public void CopyTextFile_CopiesContent()
    {
        var src = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        var dst = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");

        File.WriteAllText(src, "a\nb\n");
        Task18_CopyFileStreamReaderWriter.FileCopier.CopyTextFile(src, dst);

        Assert.Equal(File.ReadAllText(src), File.ReadAllText(dst));

        File.Delete(src);
        File.Delete(dst);
    }
}
