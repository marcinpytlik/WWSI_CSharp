-- Execute in EfPerformanceLab in SSMS after running the benchmark.
-- Query Store may need time to flush data; also run sys.sp_query_store_flush_db if appropriate.
SELECT actual_state_desc, desired_state_desc, current_storage_size_mb
FROM sys.database_query_store_options;

SELECT TOP (30)
    qt.query_sql_text,
    SUM(rs.count_executions) AS executions,
    CAST(SUM(rs.avg_duration * rs.count_executions) / NULLIF(SUM(rs.count_executions),0) / 1000.0 AS decimal(18,2)) AS avg_duration_ms,
    CAST(SUM(rs.avg_cpu_time * rs.count_executions) / NULLIF(SUM(rs.count_executions),0) / 1000.0 AS decimal(18,2)) AS avg_cpu_ms,
    CAST(SUM(rs.avg_logical_io_reads * rs.count_executions) / NULLIF(SUM(rs.count_executions),0) AS decimal(18,2)) AS avg_logical_reads_pages
FROM sys.query_store_runtime_stats AS rs
JOIN sys.query_store_plan AS p ON rs.plan_id = p.plan_id
JOIN sys.query_store_query AS q ON p.query_id = q.query_id
JOIN sys.query_store_query_text AS qt ON qt.query_text_id = q.query_text_id
WHERE qt.query_sql_text LIKE N'%Orders%' OR qt.query_sql_text LIKE N'%OrderItems%'
GROUP BY qt.query_sql_text
ORDER BY executions DESC;
