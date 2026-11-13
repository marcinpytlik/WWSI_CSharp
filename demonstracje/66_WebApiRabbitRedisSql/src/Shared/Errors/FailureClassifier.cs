using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace QueueCacheDemo.Errors;

public enum FailureKind { Permanent, Transient }

// Explicitly known transient faults are retried; malformed messages are never retried.
public static class FailureClassifier
{
    public static FailureKind Classify(Exception error)
    {
        if (error is InvalidCommandException or JsonException or RequestValidationException
            or RequestConflictException or ResourceNotFoundException
            or ArgumentException or FormatException)
            return FailureKind.Permanent;

        if (error is DependencyUnavailableException or TimeoutException or SocketException
            or System.IO.IOException)
            return FailureKind.Transient;

        if (error is SqlException sql)
        {
            // SQL Server connection, throttling, failover, deadlock and timeout errors.
            foreach (SqlError item in sql.Errors)
                if (IsTransientSqlErrorNumber(item.Number))
                    return FailureKind.Transient;

            return FailureKind.Permanent;
        }

        if (error is DbUpdateException db && db.InnerException is not null)
            return Classify(db.InnerException);

        if (error is Microsoft.EntityFrameworkCore.Storage.RetryLimitExceededException retry
            && retry.InnerException is not null)
            return Classify(retry.InnerException);

        // DbConnection/EF may wrap provider failures in InvalidOperationException.
        // Unwrap only when the inner chain contains a known SQL/provider failure.
        if (error is InvalidOperationException && error.InnerException is not null
            && ContainsSqlDependencyFailure(error.InnerException))
            return Classify(FindSqlDependencyFailure(error.InnerException)!);

        // Unexpected errors deserve bounded retries, then DLQ for investigation.
        return FailureKind.Transient;
    }
    // 11001 is a name-resolution failure (SocketException / DNS when the SQL container is stopped).
    // This is a laboratory-dependent connection fault, not a SQL query or schema error.
    public static bool IsTransientSqlErrorNumber(int number)
        => number is -2 or 1205 or 4060 or 40197 or 40501 or 40613
            or 10928 or 10929 or 49918 or 49919 or 49920
            or 0 or 2 or 26 or 40 or 53 or 64 or 233
            or 10053 or 10054 or 10060 or 10061 or 11001;

    // API-only mapping: don't turn arbitrary CLR exceptions into HTTP 503.
    public static bool IsTransientSqlDependency(Exception error)
    {
        var cause = FindSqlDependencyFailure(error);
        return cause is not null && Classify(cause) == FailureKind.Transient;
    }

    private static bool ContainsSqlDependencyFailure(Exception error)
        => FindSqlDependencyFailure(error) is not null;

    private static Exception? FindSqlDependencyFailure(Exception error)
    {
        for (Exception? current = error; current is not null; current = current.InnerException)
        {
            if (current is SqlException) return current;
        }
        return null;
    }

}
