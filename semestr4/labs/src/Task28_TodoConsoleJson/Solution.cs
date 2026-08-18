using System.Text.Json;

namespace Task28_TodoConsoleJson;

public enum TodoStatus { Todo, InProgress, Done }
public sealed record TodoItem(Guid Id, string Title, TodoStatus Status, DateTimeOffset CreatedAt);

public sealed class TodoStore
{
    private readonly string _path;
    private readonly JsonSerializerOptions _opts = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public TodoStore(string path) => _path = path;

    public List<TodoItem> Load()
    {
        if (!File.Exists(_path)) return new();
        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<List<TodoItem>>(json, _opts) ?? new();
    }

    public void Save(List<TodoItem> items)
    {
        var json = JsonSerializer.Serialize(items, _opts);
        File.WriteAllText(_path, json);
    }

    public TodoItem Add(string title, TodoStatus status)
    {
        var items = Load();
        var item = new TodoItem(Guid.NewGuid(), title, status, DateTimeOffset.UtcNow);
        items.Add(item);
        Save(items);
        return item;
    }
}
