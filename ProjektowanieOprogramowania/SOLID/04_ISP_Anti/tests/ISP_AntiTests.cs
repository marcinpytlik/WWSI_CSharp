using Solid.ISP.Anti;
using Xunit;

public class ISP_Anti_Tests
{
    [Fact]
    public void NotImplemented_Signals_Bad_Interface()
    {
        var r = new SimpleRepo();
        Assert.Throws<NotImplementedException>(() => r.BulkInsert(new[] { 1, 2 }));
    }
}
