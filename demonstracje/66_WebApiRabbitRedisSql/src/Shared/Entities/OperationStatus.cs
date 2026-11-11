namespace QueueCacheDemo.Entities;

public sealed class OperationStatus
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string PrincipalId { get; set; } = "";
    public string IdempotencyKey { get; set; } = "";
    public string RequestHash { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public string? ErrorCode { get; set; }
}
