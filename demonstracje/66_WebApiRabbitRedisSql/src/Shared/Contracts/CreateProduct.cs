namespace QueueCacheDemo.Contracts;

public sealed record CreateProduct(Guid OperationId, Guid ProductId, string Name, decimal Price, string PrincipalId, string? TraceParent = null);
