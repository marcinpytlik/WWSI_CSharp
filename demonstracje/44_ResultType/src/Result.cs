namespace Demo44;

public readonly record struct Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(bool ok, T? value, string? error)
    {
        IsSuccess = ok;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(true, value, null);
    public static Result<T> Fail(string error) => new(false, default, error);
}

public sealed record Account
{
    public int Id { get; init; }
    public decimal Balance { get; init; }
}

public static class TransferService
{
    public static Result<Account> Withdraw(Account account, decimal amount)
    {
        if (amount <= 0)
            return Result<Account>.Fail("Amount must be positive.");
        if (account.Balance < amount)
            return Result<Account>.Fail("Insufficient funds.");
        return Result<Account>.Ok(account with { Balance = account.Balance - amount });
    }
}
