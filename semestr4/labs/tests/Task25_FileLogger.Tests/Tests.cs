using Xunit;

namespace Task25_FileLogger.Tests;

public sealed class FileLoggerTests
{
    [Fact]
    public void Log_AppendsToFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".log");
        var logger = new Task25_FileLogger.FileLogger(path);

        logger.Log("a");
        logger.Log("b");

        var text = File.ReadAllText(path);
        Assert.Contains("a", text);
        Assert.Contains("b", text);

        File.Delete(path);
    }
}
