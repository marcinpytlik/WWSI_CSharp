namespace Demo32;

public interface IReportProcessor
{
    Task ProcessAsync(int reportId);
}

public sealed class ReportProcessor : IReportProcessor
{
    private readonly ReportsDb _db;

    public ReportProcessor(ReportsDb db) => _db = db;

    public async Task ProcessAsync(int reportId)
    {
        var report = await _db.Reports.FindAsync(reportId)
            ?? throw new KeyNotFoundException($"Report {reportId} not found.");
        report.Status = "Done";
        report.ProcessedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
