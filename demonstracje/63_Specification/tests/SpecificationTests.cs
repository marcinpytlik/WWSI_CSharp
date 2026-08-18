using Demo63;
using Xunit;

namespace Demo63.Tests;

public class SpecificationTests
{
    private static readonly Order[] Orders =
    [
        new() { Id = 1, Customer = "Ada", Total = 10, Paid = true },
        new() { Id = 2, Customer = "Ada", Total = 80, Paid = false },
        new() { Id = 3, Customer = "Alan", Total = 40, Paid = true }
    ];

    [Fact]
    public void PaidOrders_OnlyPaid_OrderedByTotal()
    {
        var result = SpecEvaluator.Apply(Orders, new PaidOrdersSpec());
        Assert.Equal(new[] { 1, 3 }, result.Select(o => o.Id));
    }

    [Fact]
    public void CustomerSpec_FiltersAda()
    {
        var result = SpecEvaluator.Apply(Orders, new CustomerSpec("Ada"));
        Assert.Equal(2, result.Count);
        Assert.All(result, o => Assert.Equal("Ada", o.Customer));
    }
}
