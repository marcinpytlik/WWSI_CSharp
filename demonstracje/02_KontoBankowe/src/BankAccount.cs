namespace Demo02;

public sealed class BankAccount
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Owner { get; }
    public decimal Balance { get; private set; }

    public BankAccount(string owner, decimal openingBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(owner))
            throw new ArgumentException("Owner is required.", nameof(owner));
        if (openingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance));

        Owner = owner.Trim();
        Balance = openingBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient funds.");
        Balance -= amount;
    }
}

public static class Program
{
    public static int Main()
    {
        var account = new BankAccount("Ada", 100);
        account.Deposit(40);
        account.Withdraw(25);
        Console.WriteLine($"{account.Owner}: {account.Balance:F2}");
        return 0;
    }
}
