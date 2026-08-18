using System.Collections.Concurrent;

namespace Demo45;

public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
}

public sealed record Note(Guid Id, string Title);
public sealed record CreateNoteCommand(string Title);
public sealed record ListNotesQuery;

public sealed class NoteStore
{
    public ConcurrentDictionary<Guid, Note> Items { get; } = new();
}

public sealed class CreateNoteHandler : ICommandHandler<CreateNoteCommand, Note>
{
    private readonly NoteStore _store;
    public CreateNoteHandler(NoteStore store) => _store = store;

    public Task<Note> Handle(CreateNoteCommand command, CancellationToken cancellationToken)
    {
        var note = new Note(Guid.NewGuid(), command.Title.Trim());
        _store.Items[note.Id] = note;
        return Task.FromResult(note);
    }
}

public sealed class ListNotesHandler : IQueryHandler<ListNotesQuery, IReadOnlyList<Note>>
{
    private readonly NoteStore _store;
    public ListNotesHandler(NoteStore store) => _store = store;

    public Task<IReadOnlyList<Note>> Handle(ListNotesQuery query, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Note>>(_store.Items.Values.OrderBy(n => n.Title).ToList());
}
