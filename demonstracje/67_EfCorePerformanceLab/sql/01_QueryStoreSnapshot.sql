/* 
EF Core Performance Lab 67 — snapshot QUERY STORE.
W SSMS otwórz EfPerformanceLab i ustaw @Label na BEFORE lub AFTER
(dwa uruchomienia w osobnych oknach są poprawne).
Nie uruchamiaj równolegle innych benchmarków.
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
DECLARE @Label nvarchar(30) = N'BEFORE'; -- zmień na AFTER po uruchomieniu benchmarku
DECLARE @CaptureId uniqueidentifier = NEWID();
EXEC sys.sp_query_store_flush_db;
INSERT dbo.EfLabQueryStoreSnapshots
    (CaptureId, Label, CapturedUtc, QueryId, PlanId, Tag, Executions,
     TotalDurationUs, TotalCpuUs, TotalLogicalReads)
SELECT @CaptureId, @Label, SYSUTCDATETIME(), q.query_id, p.plan_id,
       CASE
           WHEN qt.query_sql_text LIKE N'%EFPLAB:NPLUSONE:ORDERS%' THEN N'NPLUSONE:ORDERS'
           WHEN qt.query_sql_text LIKE N'%EFPLAB:NPLUSONE:ITEMS%' THEN N'NPLUSONE:ITEMS'
           WHEN qt.query_sql_text LIKE N'%EFPLAB:INCLUDE%' THEN N'INCLUDE'
           WHEN qt.query_sql_text LIKE N'%EFPLAB:PROJECTION%' THEN N'PROJECTION'
           WHEN qt.query_sql_text LIKE N'%EFPLAB:SPLIT%' THEN N'SPLIT'
       END,
       SUM(CONVERT(bigint, rs.count_executions)),
       SUM(CONVERT(float, rs.avg_duration) * rs.count_executions),
       SUM(CONVERT(float, rs.avg_cpu_time) * rs.count_executions),
       SUM(CONVERT(float, rs.avg_logical_io_reads) * rs.count_executions)
FROM sys.query_store_query_text AS qt
JOIN sys.query_store_query AS q ON q.query_text_id = qt.query_text_id
JOIN sys.query_store_plan AS p ON p.query_id = q.query_id
JOIN sys.query_store_runtime_stats AS rs ON rs.plan_id = p.plan_id
WHERE qt.query_sql_text LIKE N'%EFPLAB:%'
  AND rs.execution_type = 0
GROUP BY q.query_id, p.plan_id, qt.query_sql_text;
SELECT @CaptureId AS CaptureId, @Label AS Label, COUNT(*) AS SnapshotRows
FROM dbo.EfLabQueryStoreSnapshots WHERE CaptureId = @CaptureId;
GO
