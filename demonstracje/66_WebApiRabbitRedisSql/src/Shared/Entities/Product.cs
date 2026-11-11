namespace QueueCacheDemo.Entities;

public sealed class Product
{
    public Guid Id { get; set; }
    public string PrincipalId { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
