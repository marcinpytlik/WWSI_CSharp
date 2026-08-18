using System.Collections.Concurrent;

namespace Demo62;

public interface IRequest<TResponse>;

public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _services;
    public Mediator(IServiceProvider services) => _services = services;

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = _services.GetRequiredService(handlerType);
        return handler.Handle((dynamic)request, cancellationToken);
    }
}

public sealed record Note(Guid Id, string Title);
public sealed record CreateNote(string Title) : IRequest<Note>;
public sealed record ListNotes : IRequest<IReadOnlyList<Note>>;

public sealed class NoteStore
{
    public ConcurrentDictionary<Guid, Note> Items { get; } = new();
}

public sealed class CreateNoteHandler : IRequestHandler<CreateNote, Note>
{
    private readonly NoteStore _store;
    public CreateNoteHandler(NoteStore store) => _store = store;

    public Task<Note> Handle(CreateNote request, CancellationToken cancellationToken)
    {
        var note = new Note(Guid.NewGuid(), request.Title.Trim());
        _store.Items[note.Id] = note;
        return Task.FromResult(note);
    }
}

public sealed class ListNotesHandler : IRequestHandler<ListNotes, IReadOnlyList<Note>>
{
    private readonly NoteStore _store;
    public ListNotesHandler(NoteStore store) => _store = store;

    public Task<IReadOnlyList<Note>> Handle(ListNotes request, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Note>>(_store.Items.Values.OrderBy(n => n.Title).ToList());
}
