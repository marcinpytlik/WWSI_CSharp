/*
EF Core Performance Lab 67: dla jednego tokena z Run-All-Benchmark.ps1
porównaj cztery niezależne pary BEFORE/AFTER.
Wynik w stabilnym formacie EFPLAB_RESULT|... służy do importu przez PS.
Ten skrypt niczego nie modyfikuje.
*/
USE EfPerformanceLab;
GO
DECLARE @Token nvarchar(8) = N'__RUN_TOKEN__';

;WITH Strategies AS
(
    SELECT Strategy
    FROM (VALUES (N'nplusone'), (N'include'), (N'projection'), (N'split')) AS s(Strategy)
),
Captures AS
(
    SELECT s.Strategy, b.CaptureId AS BeforeId, a.CaptureId AS AfterId
    FROM Strategies AS s
    OUTER APPLY
    (
        SELECT TOP (1) CaptureId, MAX(CapturedUtc) AS CapturedUtc
        FROM dbo.EfLabQueryStoreSnapshots
        WHERE Label = N'BEFORE:' + s.Strategy + N':' + @Token
        GROUP BY CaptureId ORDER BY MAX(CapturedUtc) DESC
    ) AS b
    OUTER APPLY
    (
        SELECT TOP (1) CaptureId, MAX(CapturedUtc) AS CapturedUtc
        FROM dbo.EfLabQueryStoreSnapshots
        WHERE Label = N'AFTER:' + s.Strategy + N':' + @Token
        GROUP BY CaptureId ORDER BY MAX(CapturedUtc) DESC
    ) AS a
    WHERE b.CaptureId IS NOT NULL
      AND a.CaptureId IS NOT NULL
      AND a.CapturedUtc > b.CapturedUtc
),
Deltas AS
(
    SELECT c.Strategy, afters.Tag,
           afters.Executions - ISNULL(befores.Executions, 0) AS Executions,
           afters.TotalDurationUs - ISNULL(befores.TotalDurationUs, 0) AS DurationUs,
           afters.TotalCpuUs - ISNULL(befores.TotalCpuUs, 0) AS CpuUs,
           afters.TotalLogicalReads - ISNULL(befores.TotalLogicalReads, 0) AS LogicalReads
    FROM Captures AS c
    JOIN dbo.EfLabQueryStoreSnapshots AS afters
      ON afters.CaptureId = c.AfterId
    LEFT JOIN dbo.EfLabQueryStoreSnapshots AS befores
      ON befores.CaptureId = c.BeforeId
     AND befores.QueryId = afters.QueryId
     AND befores.PlanId = afters.PlanId
    WHERE (c.Strategy = N'nplusone' AND afters.Tag IN (N'SHARED:ORDERS', N'NPLUSONE:ITEMS'))
       OR (c.Strategy = N'include' AND afters.Tag = N'INCLUDE')
       OR (c.Strategy = N'projection' AND afters.Tag = N'PROJECTION')
       OR (c.Strategy = N'split' AND afters.Tag IN (N'SHARED:ORDERS', N'SPLIT:ITEMS'))
)
SELECT CONCAT(
    'EFPLAB_RESULT|', Strategy, '|',
    SUM(Executions), '|',
    CONVERT(varchar(50), CAST(SUM(DurationUs) / 1000.0 AS decimal(18,3))), '|',
    CONVERT(varchar(50), CAST(SUM(CpuUs) / 1000.0 AS decimal(18,3))), '|',
    CONVERT(varchar(50), CAST(SUM(LogicalReads) AS decimal(18,0)))
) AS CsvRecord
FROM Deltas
GROUP BY Strategy
ORDER BY CASE Strategy WHEN N'nplusone' THEN 1 WHEN N'include' THEN 2
                       WHEN N'projection' THEN 3 ELSE 4 END;
GO
