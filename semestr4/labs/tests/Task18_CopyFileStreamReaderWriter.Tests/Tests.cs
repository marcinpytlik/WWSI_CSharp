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

        // StreamWriter uses Environment.NewLine, so compare lines rather than raw bytes.
        Assert.Equal(File.ReadAllLines(src), File.ReadAllLines(dst));

        File.Delete(src);
        File.Delete(dst);
    }
}
