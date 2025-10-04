
using System.Diagnostics;

namespace Advanced.ErrorsDiagnostics;

public record Problem(string Title, string Detail, int Status, string TraceId);

public static class Diagnostics
{
    public static Problem ToProblem(this Exception ex)
    {
        var activity = Activity.Current ?? new Activity("manual").Start();
        return new Problem("Error", ex.Message, 500, activity.TraceId.ToString());
    }
}
