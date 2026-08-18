using Xunit;

namespace Task03_GroupByAge.Tests;

public sealed class GrouperTests
{
    [Fact]
    public void GroupByAge_GroupsCorrectly()
    {
        var input = new[]
        {
            new Task03_GroupByAge.Person("Ala", 30),
            new Task03_GroupByAge.Person("Ola", 30),
            new Task03_GroupByAge.Person("Jan", 40),
        };

        var grouped = Task03_GroupByAge.Grouper.GroupByAge(input);

        Assert.Equal(2, grouped[30].Count);
        Assert.Single(grouped[40]);
    }
}
