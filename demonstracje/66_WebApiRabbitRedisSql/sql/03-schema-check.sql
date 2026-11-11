USE WwsiQueueCache;
GO
SELECT t.name AS TableName
FROM sys.tables t
WHERE t.name IN (N'Products', N'ProcessedCommands', N'OperationStatuses', N'OutboxMessages')
ORDER BY t.name;
SELECT i.name AS IndexName, OBJECT_NAME(i.object_id) AS TableName, i.is_unique
FROM sys.indexes i
WHERE i.name IN (N'UX_OperationStatuses_Principal_Key', N'UX_OutboxMessages_OperationId', N'IX_OutboxMessages_PublishedAtUtc')
ORDER BY TableName;
SELECT Status, COUNT(*) AS Operations
FROM dbo.OperationStatuses GROUP BY Status;
SELECT COUNT(*) AS Unpublished, MIN(CreatedAtUtc) AS OldestUnpublished
FROM dbo.OutboxMessages WHERE PublishedAtUtc IS NULL;
GO
