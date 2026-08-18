using Xunit;

namespace Task09_WriteUserJson.Tests;

public sealed class UserJsonWriterTests
{
    [Fact]
    public async Task WriteAsync_WritesAndReadsBack()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        var user = new Task09_WriteUserJson.User(1, "u", "e@example.local");

        await Task09_WriteUserJson.UserJsonWriter.WriteAsync(user, path);
        var back = await Task09_WriteUserJson.UserJsonWriter.ReadAsync(path);

        Assert.Equal(user, back);
        File.Delete(path);
    }
}
