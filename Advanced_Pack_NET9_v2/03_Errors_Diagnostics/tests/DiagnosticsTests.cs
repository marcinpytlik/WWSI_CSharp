
using Advanced.ErrorsDiagnostics;
using System.Diagnostics;
using Xunit;

public class DiagnosticsTests
{
    [Fact]
    public void Exception_To_Problem_Has_TraceId()
    {
        using var a = new Activity("test").Start();
        var p = new InvalidOperationException("boom").ToProblem();
        Assert.Equal("boom", p.Detail);
        Assert.False(string.IsNullOrWhiteSpace(p.TraceId));
    }
}
