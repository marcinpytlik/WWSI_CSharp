/*
EF Core Performance Lab 67 — delta snapshotów.
Wykorzystuje najnowsze BEFORE i AFTER; nie resetuje Query Store.
Warunek: osobna seria pomiarowa bez innych klientów.
*/
USE EfPerformanceLab;
GO
;WITH Captures AS
(
    SELECT CaptureId, Label, MAX(CapturedUtc) AS CapturedUtc,
           ROW_NUMBER() OVER (PARTITION BY Label ORDER BY MAX(CapturedUtc) DESC) AS rn
    FROM dbo.EfLabQueryStoreSnapshots
    WHERE Label IN (N'BEFORE', N'AFTER')
    GROUP BY CaptureId, Label
),
B AS
(
    SELECT s.* FROM dbo.EfLabQueryStoreSnapshots AS s
    JOIN Captures AS c ON c.CaptureId = s.CaptureId AND c.Label = N'BEFORE' AND c.rn = 1
),
A AS
(
    SELECT s.* FROM dbo.EfLabQueryStoreSnapshots AS s
    JOIN Captures AS c ON c.CaptureId = s.CaptureId AND c.Label = N'AFTER' AND c.rn = 1
),
Delta AS
(
    SELECT a.Tag,
           a.Executions - ISNULL(b.Executions, 0) AS Executions,
           a.TotalDurationUs - ISNULL(b.TotalDurationUs, 0) AS DurationUs,
           a.TotalCpuUs - ISNULL(b.TotalCpuUs, 0) AS CpuUs,
           a.TotalLogicalReads - ISNULL(b.TotalLogicalReads, 0) AS LogicalReads
    FROM A AS a
    LEFT JOIN B AS b ON b.QueryId = a.QueryId AND b.PlanId = a.PlanId
)
SELECT Tag,
       SUM(Executions) AS Executions,
       CAST(SUM(DurationUs) / 1000.0 AS decimal(18,2)) AS TotalDurationMs,
       CAST(SUM(CpuUs) / 1000.0 AS decimal(18,2)) AS TotalCpuMs,
       CAST(SUM(LogicalReads) AS decimal(18,0)) AS TotalLogicalReads,
       CAST(SUM(LogicalReads) / NULLIF(SUM(Executions),0) AS decimal(18,2)) AS LogicalReadsPerExecution
FROM Delta
GROUP BY Tag
ORDER BY Tag;
GO
-- Weryfikacja: przy 10 powtórzeniach / take=100 NPLUSONE:ITEMS ≈ 1000 wykonań,
-- NPLUSONE:ORDERS ≈ 10, INCLUDE ≈ 10, PROJECTION ≈ 10, SPLIT ≈ 20 (dwa SELECT-y).
-- Query Store może opóźniać agregację; analizuj wyniki po flush obu snapshotów.
-- Uwaga: SplitQuery może mieć więcej niż jeden QueryId pod wspólnym Tag.
