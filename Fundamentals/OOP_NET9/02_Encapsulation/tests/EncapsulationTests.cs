using Oop.Encapsulation;
using Xunit;

public class EncapsulationTests
{
    [Fact]
    public void Deposit_Increases_Balance()
    {
        var a = new BankAccount();
        a.Deposit(10);
        Assert.Equal(10, a.Balance);
    }
}
