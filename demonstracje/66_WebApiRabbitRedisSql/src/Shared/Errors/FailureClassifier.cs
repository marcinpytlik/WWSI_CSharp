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
                if (item.Number is -2 or 1205 or 4060 or 40197 or 40501 or 40613
                    or 10928 or 10929 or 49918 or 49919 or 49920
                    or 233 or 10053 or 10054 or 10060)
                    return FailureKind.Transient;

            return FailureKind.Permanent;
        }

        if (error is DbUpdateException db && db.InnerException is not null)
            return Classify(db.InnerException);

        // Unexpected errors deserve bounded retries, then DLQ for investigation.
        return FailureKind.Transient;
    }
}
