
namespace Orders.Rich.Domain.Primitives;

public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money Zero(string c = "PLN") => new(0m, c);
    public static Money operator +(Money a, Money b)
        => a.Currency == b.Currency ? new Money(a.Amount + b.Amount, a.Currency)
                                    : throw new InvalidOperationException("Currency mismatch");
    public static Money operator *(Money a, decimal factor) => new(a.Amount * factor, a.Currency);
    public Money Round2() => new(Math.Round(Amount, 2), Currency);
    public override string ToString() => $"{Amount:0.00} {Currency}";
}
