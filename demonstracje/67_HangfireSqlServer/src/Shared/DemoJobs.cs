using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Demo67.Shared;

public sealed class DemoJobs(string connectionString, ILogger<DemoJobs> logger)
{
    public async Task ExecuteAsync(string runKey, string kind, int seconds)
    {
        if (seconds is < 0 or > 60) throw new ArgumentOutOfRangeException(nameof(seconds));
        if (string.IsNullOrWhiteSpace(runKey) || runKey.Length > 120)
            throw new ArgumentException("Invalid run key", nameof(runKey));
        if (seconds > 0) await Task.Delay(TimeSpan.FromSeconds(seconds));
        await using var sql = new SqlConnection(connectionString);
        await sql.OpenAsync();
        await using var cmd = sql.CreateCommand();
        cmd.CommandText = """
            SET XACT_ABORT ON;
            BEGIN TRAN;
            IF NOT EXISTS (SELECT 1 FROM dbo.JobExecutions WITH (UPDLOCK, HOLDLOCK) WHERE RunKey=@key)
                INSERT dbo.JobExecutions(RunKey, Kind, ExecutedAtUtc)
                VALUES (@key, @kind, SYSUTCDATETIME());
            COMMIT;
            """;
        cmd.Parameters.AddWithValue("@key", runKey);
        cmd.Parameters.AddWithValue("@kind", kind);
        await cmd.ExecuteNonQueryAsync();
        logger.LogInformation("DEMO67 EXECUTED RunKey={Key} Kind={Kind}",runKey,kind);
    }

    public Task RecurringHeartbeatAsync() => ExecuteAsync($"heartbeat-{Guid.NewGuid():N}", "recurring", 0);

    public async Task FailOnceAsync(string runKey)
    {
        await using var sql = new SqlConnection(connectionString);
        await sql.OpenAsync();
        await using var cmd = sql.CreateCommand();
        cmd.CommandText = """
            SET XACT_ABORT ON;
            BEGIN TRAN;
            UPDATE dbo.JobAttempts WITH (UPDLOCK,HOLDLOCK)
            SET Attempts=Attempts+1 WHERE RunKey=@key;
            IF @@ROWCOUNT=0 INSERT dbo.JobAttempts(RunKey,Attempts) VALUES (@key,1);
            SELECT Attempts FROM dbo.JobAttempts WHERE RunKey=@key;
            COMMIT;
            """;
        cmd.Parameters.AddWithValue("@key",runKey);
        var attempt = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        if (attempt == 1)
        {
            logger.LogWarning("DEMO67 INTENTIONAL FAILURE RunKey={Key}",runKey);
            throw new InvalidOperationException("Demo67 intentional first-attempt failure");
        }
        await ExecuteAsync(runKey, "retry-recovered", 0);
    }
}
