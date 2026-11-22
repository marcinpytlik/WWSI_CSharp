/*
EF Core Performance Lab 67 — porównanie snapshotów BEFORE / AFTER.
Uruchom jedną strategię między snapshotami. Gdy Query Store łączy
identyczne zapytania (N+1:Orders i Split:Orders), odrębny przebieg
umożliwia prawidłową interpretację. Żaden wynik nie jest zerowany.
*/
USE EfPerformanceLab;
GO
IF OBJECT_ID(N'dbo.EfLabQueryStoreSnapshots', N'U') IS NULL
    THROW 51068, 'Missing snapshot table. Execute 01_QueryStoreSnapshot.sql.', 1;
GO
DECLARE @Before uniqueidentifier, @After uniqueidentifier;
DECLARE @BeforeUtc datetime2(3), @AfterUtc datetime2(3);

SELECT TOP (1) @Before = CaptureId, @BeforeUtc = MAX(CapturedUtc)
FROM dbo.EfLabQueryStoreSnapshots
WHERE Label = N'BEFORE'
GROUP BY CaptureId ORDER BY MAX(CapturedUtc) DESC;

SELECT TOP (1) @After = CaptureId, @AfterUtc = MAX(CapturedUtc)
FROM dbo.EfLabQueryStoreSnapshots
WHERE Label = N'AFTER'
GROUP BY CaptureId ORDER BY MAX(CapturedUtc) DESC;

IF @Before IS NULL OR @After IS NULL
    THROW 51069, 'Missing BEFORE or AFTER snapshot.', 1;
IF @AfterUtc <= @BeforeUtc
    THROW 51070, 'AFTER must be newer than BEFORE. Take snapshots in correct order.', 1;

SELECT @Before AS BeforeCaptureId, @BeforeUtc AS BeforeUtc,
       @After AS AfterCaptureId, @AfterUtc AS AfterUtc;

;WITH B AS
(
    SELECT * FROM dbo.EfLabQueryStoreSnapshots WHERE CaptureId = @Before
),
A AS
(
    SELECT * FROM dbo.EfLabQueryStoreSnapshots WHERE CaptureId = @After
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
SELECT Tag, SUM(Executions) AS Executions,
       CAST(SUM(DurationUs) / 1000.0 AS decimal(18,2)) AS TotalDurationMs,
       CAST(SUM(CpuUs) / 1000.0 AS decimal(18,2)) AS TotalCpuMs,
       CAST(SUM(LogicalReads) AS decimal(18,0)) AS TotalLogicalReads,
       CAST(SUM(LogicalReads) / NULLIF(SUM(Executions), 0) AS decimal(18,2))
           AS LogicalReadsPerExecution
FROM Delta
GROUP BY Tag
HAVING SUM(Executions) > 0
ORDER BY Tag;
GO
-- Dla take=100, Repetitions=10, SkipWarmup i wyłącznego obciążenia:
-- N+1 => SHARED:ORDERS ~10; NPLUSONE:ITEMS ~1000
-- Include => INCLUDE ~10
-- Projection => PROJECTION ~10
-- Split => SHARED:ORDERS ~10; SPLIT:ITEMS ~10
-- Różnice mogą obejmować inne operacje z tych samych QueryId;
-- zagwarantuj brak współbieżnego ruchu dla porównywalności.
