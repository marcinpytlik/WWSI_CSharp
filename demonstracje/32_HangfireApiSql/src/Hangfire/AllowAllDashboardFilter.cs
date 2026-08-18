using Hangfire.Dashboard;

namespace Demo32.HangfireHost;

public sealed class AllowAllDashboardFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}
