using Demo11;

var sku = args.Length > 0 ? args[0] : "SKU-42";
var qty = args.Length > 1 && int.TryParse(args[1], out var parsed) ? parsed : 2;

var options = new RabbitOptions();
await using var publisher = new RabbitPublisher(options);
var message = new OrderPlaced(Guid.NewGuid(), sku, qty);
await publisher.PublishAsync(message);

Console.WriteLine($"Published {message.OrderId} {message.Sku} x{message.Qty} -> {options.Queue}");
