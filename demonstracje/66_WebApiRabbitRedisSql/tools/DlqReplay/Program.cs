using System.Text.Json;
using QueueCacheDemo.Contracts;
using RabbitMQ.Client;

const string DeadQueue = "products.dead";
const string MainQueue = "products.create";
const int ScanLimit = 100;

// Operator CLI: inspect with BasicGet (manual ACK), and requeue all unselected deliveries.
// Replay is restricted to valid, test-created DeadLetterProbe messages.
if (args.Length < 1 || args[0] is not ("preview" or "replay"))
{
    Console.Error.WriteLine("Usage: preview [operation-guid] | replay <operation-guid> --approve <operation-guid>");
    return 2;
}
var mode = args[0];
if (mode == "replay" && (args.Length != 4 || args[2] != "--approve" ||
    !Guid.TryParse(args[1], out var target) ||
    !Guid.TryParse(args[3], out var approved) || target != approved))
{
    Console.Error.WriteLine("Replay requires matching --approve <operation-guid>.");
    return 2;
}
Guid? filter = null;
if (args.Length >= 2)
{
    if (!Guid.TryParse(args[1], out var id)) { Console.Error.WriteLine("Invalid operation GUID"); return 2; }
    filter = id;
}

var factory = new ConnectionFactory
{
    HostName = Environment.GetEnvironmentVariable("Rabbit__Host") ?? "rabbitmq",
    UserName = Environment.GetEnvironmentVariable("Rabbit__User") ?? throw new InvalidOperationException("Rabbit__User missing"),
    Password = Environment.GetEnvironmentVariable("Rabbit__Password") ?? throw new InvalidOperationException("Rabbit__Password missing")
};
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync(
    new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true));

// Never publish to a missing destination or read from a non-existent source.
await channel.QueueDeclarePassiveAsync(DeadQueue);
await channel.QueueDeclarePassiveAsync(MainQueue);

var held = new List<ulong>();
ulong? replayTag = null;
bool replayPublished = false;
try
{
    for (var i = 0; i < ScanLimit; i++)
    {
        var item = await channel.BasicGetAsync(DeadQueue, autoAck: false);
        if (item is null) break;
        held.Add(item.DeliveryTag);
        CreateProduct? command;
        try { command = JsonSerializer.Deserialize<CreateProduct>(item.Body.Span); }
        catch (JsonException) { command = null; }

        var op = command?.OperationId;
        if (mode == "preview")
        {
            if (filter is null || op == filter)
                Console.WriteLine($"DLQ operation={op?.ToString() ?? "<invalid-json>"} messageId={item.BasicProperties.MessageId ?? "<none>"} valid={(command is not null)} name={command?.Name ?? "<invalid>"}");
            continue;
        }

        if (op != filter) continue;
        if (command is null || command.Name?.StartsWith("DeadLetterProbe-", StringComparison.Ordinal) != true
            || command.ProductId == Guid.Empty || string.IsNullOrWhiteSpace(command.PrincipalId))
        {
            Console.Error.WriteLine("Selected message failed laboratory replay validation; source remains in DLQ.");
            return 3;
        }
        Console.WriteLine($"SELECTED operation={op} name={command.Name}");
        // Publish first with durable delivery + tracked publisher confirmation.
        // Do not copy x-death, x-demo-attempt or other old delivery headers.
        await channel.BasicPublishAsync(exchange: "", routingKey: MainQueue, mandatory: true,
            basicProperties: new BasicProperties { Persistent = true, MessageId = op.ToString() },
            body: item.Body);
        replayPublished = true;
        replayTag = item.DeliveryTag;
        Console.WriteLine($"PUBLISH_CONFIRMED operation={op}");
        await channel.BasicAckAsync(item.DeliveryTag, multiple: false);
        held.Remove(item.DeliveryTag);
        Console.WriteLine($"SOURCE_ACKED operation={op}");
        break;
    }
    if (mode == "replay" && !replayPublished)
    {
        Console.Error.WriteLine($"Target operation {filter} not found in first {ScanLimit} DLQ messages.");
        return 4;
    }
    if (mode == "preview") Console.WriteLine($"PREVIEW_COMPLETE scanned={held.Count} limit={ScanLimit}");
    return 0;
}
finally
{
    // No ACK for untouched entries. Preserve them for an operator's later analysis.
    // If publish succeeded but source ACK failed, duplication is possible and safe
    // for the idempotent consumer; never ACK before the publisher confirm.
    foreach (var tag in held)
    {
        try { await channel.BasicNackAsync(tag, multiple: false, requeue: true); }
        catch (Exception ex) { Console.Error.WriteLine($"WARNING requeue failed for delivery={tag}: {ex.Message}"); }
    }
}
