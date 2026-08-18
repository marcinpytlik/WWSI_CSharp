using Xunit;

namespace Task29_TodoFilterByStatus.Tests;

public sealed class TodoFilterTests
{
    [Fact]
    public void FilterByStatus_ReturnsMatches()
    {
        var items = new[]
        {
            new Task29_TodoFilterByStatus.TodoItem(Guid.NewGuid(), "A", Task29_TodoFilterByStatus.TodoStatus.Done),
            new Task29_TodoFilterByStatus.TodoItem(Guid.NewGuid(), "B", Task29_TodoFilterByStatus.TodoStatus.Todo),
        };

        var done = Task29_TodoFilterByStatus.TodoFilter.FilterByStatus(items, Task29_TodoFilterByStatus.TodoStatus.Done);
        Assert.Single(done);
        Assert.Equal("A", done[0].Title);
    }
}
