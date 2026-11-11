namespace QueueCacheDemo.Entities;

public sealed class ProcessedCommand
{
    public Guid Id { get; set; }
    public DateTime CompletedAtUtc { get; set; }
}
