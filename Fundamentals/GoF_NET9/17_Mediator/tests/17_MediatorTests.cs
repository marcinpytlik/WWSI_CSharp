using GoF.Mediator;
using Xunit;

public class MediatorTests
{
    [Fact]
    public void Publishes_Messages()
    {
        var m = new SimpleMediator();
        object? seen = null;
        m.Subscribe("topic", o => seen = o);
        m.Publish("topic", 42);
        Assert.Equal(42, seen);
    }
}
