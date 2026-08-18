namespace Demo11;

public sealed class OrderRecord
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = "";
    public int Qty { get; set; }
    public DateTime ReceivedUtc { get; set; }
}
