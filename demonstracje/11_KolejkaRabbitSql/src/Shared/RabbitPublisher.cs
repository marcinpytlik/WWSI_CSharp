using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Demo11;

public sealed class RabbitPublisher : IAsyncDisposable
{
    private readonly RabbitOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitPublisher(RabbitOptions options) => _options = options;

    public async Task PublishAsync(OrderPlaced message, CancellationToken cancellationToken = default)
    {
        await EnsureChannelAsync(cancellationToken);
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        await _channel!.BasicPublishAsync(
            exchange: "",
            routingKey: _options.Queue,
            body: body,
            cancellationToken: cancellationToken);
    }

    private async Task EnsureChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true }) return;

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.User,
            Password = _options.Password
        };
        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.QueueDeclareAsync(
            _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
