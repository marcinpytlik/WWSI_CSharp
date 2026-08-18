using Xunit;

namespace Task07_WordCount.Tests;

public sealed class WordCounterTests
{
    [Fact]
    public void CountWordsFromFile_Counts()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        File.WriteAllText(path, "Ala ma kota. Ala ma psa.");

        var dict = Task07_WordCount.WordCounter.CountWordsFromFile(path);

        Assert.Equal(2, dict["ala"]);
        Assert.Equal(2, dict["ma"]);
        Assert.Equal(1, dict["kota"]);
        File.Delete(path);
    }
}
