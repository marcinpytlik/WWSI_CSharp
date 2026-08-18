using Demo60;
using Grpc.Net.Client;

namespace Demo60.Api;

public interface INotesClient
{
    Task<NoteReply> AddAsync(string title, CancellationToken cancellationToken);
    Task<IReadOnlyList<NoteReply>> ListAsync(CancellationToken cancellationToken);
}

public sealed class GrpcNotesClient : INotesClient
{
    private readonly Notes.NotesClient _client;

    public GrpcNotesClient(IConfiguration configuration)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        var url = configuration["Grpc:Url"] ?? "http://localhost:8080";
        _client = new Notes.NotesClient(GrpcChannel.ForAddress(url));
    }

    public async Task<NoteReply> AddAsync(string title, CancellationToken cancellationToken)
        => await _client.AddAsync(new AddNoteRequest { Title = title }, cancellationToken: cancellationToken);

    public async Task<IReadOnlyList<NoteReply>> ListAsync(CancellationToken cancellationToken)
    {
        var list = await _client.ListAsync(new Empty(), cancellationToken: cancellationToken);
        return list.Items;
    }
}
