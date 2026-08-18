namespace Task29_TodoFilterByStatus;

public enum TodoStatus { Todo, InProgress, Done }
public sealed record TodoItem(Guid Id, string Title, TodoStatus Status);

public static class TodoFilter
{
    public static IReadOnlyList<TodoItem> FilterByStatus(IEnumerable<TodoItem> items, TodoStatus status)
        => items.Where(x => x.Status == status).ToList();
}
