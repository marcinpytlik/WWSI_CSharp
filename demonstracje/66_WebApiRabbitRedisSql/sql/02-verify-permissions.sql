USE [WwsiQueueCache];
GO
-- Execute as sa from SSMS/sqlcmd. 1 = permitted, 0 = not permitted.
EXECUTE AS USER = N'wwsi_api';
SELECT USER_NAME() AS PrincipalName,
       HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'SELECT') AS ProductsSelect,
       HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'INSERT') AS ProductsInsert,
       HAS_PERMS_BY_NAME(N'dbo.ProcessedCommands', N'OBJECT', N'SELECT') AS CommandsSelect,
       HAS_PERMS_BY_NAME(N'dbo.ProcessedCommands', N'OBJECT', N'INSERT') AS CommandsInsert;
REVERT;
GO
EXECUTE AS USER = N'wwsi_worker';
SELECT USER_NAME() AS PrincipalName,
       HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'SELECT') AS ProductsSelect,
       HAS_PERMS_BY_NAME(N'dbo.Products', N'OBJECT', N'INSERT') AS ProductsInsert,
       HAS_PERMS_BY_NAME(N'dbo.ProcessedCommands', N'OBJECT', N'SELECT') AS CommandsSelect,
       HAS_PERMS_BY_NAME(N'dbo.ProcessedCommands', N'OBJECT', N'INSERT') AS CommandsInsert;
REVERT;
GO
