using Oop.Records;
using Xunit;

public class RecordsValueObjectsTests
{
    [Fact]
    public void Adds_Money_With_Same_Currency()
    {
        var sum = new Money(10, "PLN") + new Money(5, "PLN");
        Assert.Equal(new Money(15, "PLN"), sum);
    }
}
