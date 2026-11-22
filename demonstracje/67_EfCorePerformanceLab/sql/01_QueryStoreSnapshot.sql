/*
EF Core Performance Lab 67 — snapshot statystyk Query Store.
Użyj etykiet BEFORE i AFTER dla jednej izolowanej serii strategii.
UWAGA: Query Store może pomijać komentarze TagWith i łączyć identyczny SQL
z N+1 oraz SplitQuery. Rozpoznajemy rzeczywiste kształty instrukcji SQL,
a nie QueryId ani teksty komentarzy. Snapshot gromadzi całą historię
tych instrukcji; różnice pomiędzy snapshotami stanowią pomiar serii.
*/
USE EfPerformanceLab;
GO
IF OBJECT_ID(N'dbo.EfLabQueryStoreSnapshots', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.EfLabQueryStoreSnapshots
    (
        CaptureId uniqueidentifier NOT NULL,
        Label nvarchar(30) NOT NULL,
        CapturedUtc datetime2(3) NOT NULL,
        QueryId bigint NOT NULL,
        PlanId bigint NOT NULL,
        Tag nvarchar(40) NOT NULL,
        Executions bigint NOT NULL,
        TotalDurationUs float NOT NULL,
        TotalCpuUs float NOT NULL,
        TotalLogicalReads float NOT NULL,
        CONSTRAINT PK_EfLabQueryStoreSnapshots PRIMARY KEY (CaptureId, QueryId, PlanId)
    );
END;
GO
DECLARE @Label nvarchar(30) = N'BEFORE'; -- po benchmarku zmień na AFTER
DECLARE @CaptureId uniqueidentifier = NEWID();

EXEC sys.sp_query_store_flush_db;

;WITH Matched AS
(
    SELECT q.query_id AS QueryId, p.plan_id AS PlanId, rs.count_executions,
           rs.avg_duration, rs.avg_cpu_time, rs.avg_logical_io_reads,
           CASE
             WHEN CHARINDEX(N'FROM [OrderItems] AS [o]', qt.query_sql_text) > 0
              AND CHARINDEX(N'WHERE [o].[OrderId]', qt.query_sql_text) > 0
                 THEN N'NPLUSONE:ITEMS'
             WHEN CHARINDEX(N'SELECT TOP(@p) [o].[Id], [o].[CustomerName]', qt.query_sql_text) > 0
              AND CHARINDEX(N'FROM [Orders] AS [o]', qt.query_sql_text) > 0
              AND CHARINDEX(N'JOIN [OrderItems]', qt.query_sql_text) = 0
                 THEN N'SHARED:ORDERS'
             WHEN CHARINDEX(N'LEFT JOIN [OrderItems]', qt.query_sql_text) > 0
              AND CHARINDEX(N'[o0].[Id], [o0].[OrderId],', qt.query_sql_text) > 0
                 THEN N'INCLUDE'
             WHEN CHARINDEX(N'LEFT JOIN [OrderItems]', qt.query_sql_text) > 0
              AND CHARINDEX(N'[o0].[Id], [o0].[ProductName],', qt.query_sql_text) > 0
                 THEN N'PROJECTION'
             WHEN CHARINDEX(N'INNER JOIN [OrderItems]', qt.query_sql_text) > 0
              AND CHARINDEX(N'FROM [Orders] AS [o]', qt.query_sql_text) > 0
                 THEN N'SPLIT:ITEMS'
           END AS Tag
    FROM sys.query_store_query_text AS qt
    JOIN sys.query_store_query AS q ON q.query_text_id = qt.query_text_id
    JOIN sys.query_store_plan AS p ON p.query_id = q.query_id
    JOIN sys.query_store_runtime_stats AS rs ON rs.plan_id = p.plan_id
    WHERE rs.execution_type = 0
      -- Nie przechwytuj skryptu snapshotu: jego tekst zawiera wzorce jako literały.
      AND CHARINDEX(N'EfLabQueryStoreSnapshots', qt.query_sql_text) = 0
)
INSERT dbo.EfLabQueryStoreSnapshots
    (CaptureId, Label, CapturedUtc, QueryId, PlanId, Tag, Executions,
     TotalDurationUs, TotalCpuUs, TotalLogicalReads)
SELECT @CaptureId, @Label, SYSUTCDATETIME(), QueryId, PlanId, Tag,
       SUM(CONVERT(bigint, count_executions)),
       SUM(CONVERT(float, avg_duration) * count_executions),
       SUM(CONVERT(float, avg_cpu_time) * count_executions),
       SUM(CONVERT(float, avg_logical_io_reads) * count_executions)
FROM Matched
WHERE Tag IS NOT NULL
GROUP BY QueryId, PlanId, Tag;

SELECT @CaptureId AS CaptureId, @Label AS Label, Tag,
       COUNT(*) AS QueryPlans, SUM(Executions) AS Executions
FROM dbo.EfLabQueryStoreSnapshots
WHERE CaptureId = @CaptureId
GROUP BY Tag
ORDER BY Tag;

IF NOT EXISTS
    (SELECT 1 FROM dbo.EfLabQueryStoreSnapshots WHERE CaptureId = @CaptureId)
    THROW 51067, 'No matching EF Lab SQL statements captured. Run Test-Lab.ps1 first and check Query Store.', 1;
GO
