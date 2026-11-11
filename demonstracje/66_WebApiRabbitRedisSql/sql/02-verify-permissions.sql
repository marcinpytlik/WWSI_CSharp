USE [WwsiQueueCache];
GO
EXECUTE AS USER=N'wwsi_api';
SELECT USER_NAME() AS PrincipalName,
    HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'SELECT') AS ProductsSelect,
    HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'INSERT') AS ProductsInsert,
    HAS_PERMS_BY_NAME(N'dbo.OperationStatuses', N'OBJECT', N'SELECT') AS OperationsSelect,
    HAS_PERMS_BY_NAME(N'dbo.OperationStatuses', N'OBJECT', N'INSERT') AS OperationsInsert,
    HAS_PERMS_BY_NAME(N'dbo.OutboxMessages', N'OBJECT', N'INSERT') AS OutboxInsert,
    HAS_PERMS_BY_NAME(N'dbo.OutboxMessages', N'OBJECT', N'UPDATE') AS OutboxUpdate;
REVERT;
GO
EXECUTE AS USER=N'wwsi_worker';
SELECT USER_NAME() AS PrincipalName,
    HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'INSERT') AS ProductsInsert,
    HAS_PERMS_BY_NAME(N'dbo.OperationStatuses', N'OBJECT', N'UPDATE') AS OperationsUpdate,
    HAS_PERMS_BY_NAME(N'dbo.OutboxMessages', N'OBJECT', N'UPDATE') AS OutboxUpdate,
    HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'ALTER') AS ProductsAlter;
REVERT;
GO
