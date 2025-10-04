namespace Oop.Records;

public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money operator +(Money a, Money b)
        => a.Currency == b.Currency ? new Money(a.Amount + b.Amount, a.Currency)
                                    : throw new InvalidOperationException("Currency mismatch");
}
