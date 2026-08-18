using Demo04;
using Xunit;

namespace Demo04.Tests;

public class CsvPeopleTests
{
    [Fact]
    public void Parse_TwoRows()
    {
        var people = CsvPeople.Parse("name,year\nAda,1815\nAlan,1912\n");
        Assert.Equal(2, people.Count);
        Assert.Equal("Ada", people[0].Name);
        Assert.Equal(1912, people[1].Year);
    }

    [Fact]
    public void Parse_BadLine_Throws()
        => Assert.Throws<FormatException>(() => CsvPeople.Parse("name,year\nbad\n"));

    [Fact]
    public void ToJson_ContainsNames()
    {
        var json = CsvPeople.ToJson([new Person("Ada", 1815)]);
        Assert.Contains("Ada", json);
        Assert.Contains("1815", json);
    }
}
