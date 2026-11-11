namespace QueueCacheDemo.Entities;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public Guid OperationId { get; set; }
    public string Payload { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public Guid? LeaseOwner { get; set; }
    public DateTime? LeaseUntilUtc { get; set; }
}
