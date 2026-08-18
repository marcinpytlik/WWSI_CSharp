namespace Grasp.Refactored.Contracts;

public sealed record LineDto(string Sku, int Qty, decimal UnitPrice);

public sealed record CreateOrderDto(string Email, bool IsVip, LineDto[] Lines);
