using Xunit;

namespace Task28_TodoConsoleJson.Tests;

public sealed class TodoStoreTests
{
    [Fact]
    public void Add_PersistsToJson()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        var store = new Task28_TodoConsoleJson.TodoStore(path);

        store.Add("A", Task28_TodoConsoleJson.TodoStatus.Todo);
        store.Add("B", Task28_TodoConsoleJson.TodoStatus.Done);

        var items = store.Load();
        Assert.Equal(2, items.Count);

        File.Delete(path);
    }
}
