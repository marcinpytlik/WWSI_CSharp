using Demo02;
using Xunit;

namespace Demo02.Tests;

public class BankAccountTests
{
    [Fact]
    public void Deposit_IncreasesBalance()
    {
        var acc = new BankAccount("Ada", 10);
        acc.Deposit(5);
        Assert.Equal(15, acc.Balance);
    }

    [Fact]
    public void Withdraw_TooMuch_Throws()
    {
        var acc = new BankAccount("Ada", 10);
        Assert.Throws<InvalidOperationException>(() => acc.Withdraw(11));
        Assert.Equal(10, acc.Balance);
    }

    [Fact]
    public void EmptyOwner_Throws()
        => Assert.Throws<ArgumentException>(() => new BankAccount("  "));
}
