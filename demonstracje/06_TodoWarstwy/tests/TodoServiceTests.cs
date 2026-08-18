using Demo06;
using Xunit;

namespace Demo06.Tests;

public class TodoServiceTests
{
    [Fact]
    public async Task Add_Then_Complete_Persists()
    {
        var path = Path.Combine(Path.GetTempPath(), $"demo06-{Guid.NewGuid():N}.json");
        try
        {
            var svc = new TodoService(new JsonTodoRepository(path));
            var item = await svc.AddAsync("Lab");
            await svc.CompleteAsync(item.Id);
            var all = await new JsonTodoRepository(path).AllAsync();
            Assert.True(all.Single().Done);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Fact]
    public async Task EmptyTitle_Throws()
    {
        var svc = new TodoService(new JsonTodoRepository(Path.GetTempFileName()));
        await Assert.ThrowsAsync<ArgumentException>(() => svc.AddAsync(" "));
    }
}
