using Hangfire;

namespace Demo32.Api;

public sealed class HangfireJobQueue : IJobQueue
{
    private readonly IBackgroundJobClient _jobs;

    public HangfireJobQueue(IBackgroundJobClient jobs) => _jobs = jobs;

    public string EnqueueReport(int reportId)
        => _jobs.Enqueue<IReportProcessor>(processor => processor.ProcessAsync(reportId));
}
