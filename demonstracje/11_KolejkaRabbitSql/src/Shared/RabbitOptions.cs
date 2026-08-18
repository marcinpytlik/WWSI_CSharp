namespace Demo11;

public sealed class RabbitOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string User { get; set; } = "demo";
    public string Password { get; set; } = "demo";
    public string Queue { get; set; } = "orders.placed";
}
