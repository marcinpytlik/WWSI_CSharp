using Grpc.Core;

namespace Demo60;

public sealed class NotesGrpcService : Notes.NotesBase
{
    private readonly NotesApp _app;
    public NotesGrpcService(NotesApp app) => _app = app;

    public override async Task<NoteReply> Add(AddNoteRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length < 3)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Title must have at least 3 characters."));
        var note = await _app.AddAsync(request.Title, context.CancellationToken);
        return new NoteReply { Id = note.Id, Title = note.Title };
    }

    public override async Task<NoteList> List(Empty request, ServerCallContext context)
    {
        var list = new NoteList();
        foreach (var note in await _app.ListAsync(context.CancellationToken))
            list.Items.Add(new NoteReply { Id = note.Id, Title = note.Title });
        return list;
    }
}
