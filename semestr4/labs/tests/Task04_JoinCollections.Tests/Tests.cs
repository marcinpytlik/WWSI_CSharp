using Xunit;

namespace Task04_JoinCollections.Tests;

public sealed class JoinerTests
{
    [Fact]
    public void JoinCustomersOrders_JoinsCorrectly()
    {
        var customers = new[] { new Task04_JoinCollections.Customer(1,"A"), new Task04_JoinCollections.Customer(2,"B") };
        var orders = new[] { new Task04_JoinCollections.Order(10,1), new Task04_JoinCollections.Order(20,1), new Task04_JoinCollections.Order(30,2) };

        var result = Task04_JoinCollections.Joiner.JoinCustomersOrders(customers, orders);

        Assert.Equal(3, result.Count);
        Assert.Contains(result, x => x.CustomerName == "A" && x.OrderId == 10);
    }
}
