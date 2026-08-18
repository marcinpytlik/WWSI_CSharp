using System.Text.Json;

namespace Demo06;

public sealed class TodoItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = "";
    public bool Done { get; set; }
}

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> AllAsync();
    Task SaveAllAsync(IEnumerable<TodoItem> items);
}

public sealed class JsonTodoRepository : ITodoRepository
{
    private readonly string _path;

    public JsonTodoRepository(string path) => _path = path;

    public async Task<IReadOnlyList<TodoItem>> AllAsync()
    {
        if (!File.Exists(_path)) return [];
        await using var stream = File.OpenRead(_path);
        return await JsonSerializer.DeserializeAsync<List<TodoItem>>(stream) ?? [];
    }

    public async Task SaveAllAsync(IEnumerable<TodoItem> items)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path) ?? ".");
        await using var stream = File.Create(_path);
        await JsonSerializer.SerializeAsync(stream, items.ToList());
    }
}

public sealed class TodoService
{
    private readonly ITodoRepository _repo;

    public TodoService(ITodoRepository repo) => _repo = repo;

    public async Task<TodoItem> AddAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        var items = (await _repo.AllAsync()).ToList();
        var item = new TodoItem { Title = title.Trim() };
        items.Add(item);
        await _repo.SaveAllAsync(items);
        return item;
    }

    public async Task CompleteAsync(Guid id)
    {
        var items = (await _repo.AllAsync()).ToList();
        var item = items.SingleOrDefault(x => x.Id == id)
            ?? throw new KeyNotFoundException(id.ToString());
        item.Done = true;
        await _repo.SaveAllAsync(items);
    }
}

public static class Program
{
    public static async Task<int> Main()
    {
        var path = Path.Combine(Path.GetTempPath(), "demo06-todo.json");
        var svc = new TodoService(new JsonTodoRepository(path));
        var item = await svc.AddAsync("Prepare lecture");
        Console.WriteLine($"Added {item.Id}: {item.Title}");
        return 0;
    }
}
