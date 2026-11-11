:ON ERROR EXIT
USE [master];
GO
IF DB_ID(N'WwsiQueueCache') IS NULL
    CREATE DATABASE [WwsiQueueCache];
GO

-- Only the one-off db-init service runs as sa.
IF SUSER_ID(N'wwsi_api') IS NULL
    CREATE LOGIN [wwsi_api] WITH PASSWORD = '$(ApiPassword)', CHECK_POLICY = ON;
ELSE
    ALTER LOGIN [wwsi_api] WITH PASSWORD = '$(ApiPassword)';
GO
IF SUSER_ID(N'wwsi_worker') IS NULL
    CREATE LOGIN [wwsi_worker] WITH PASSWORD = '$(WorkerPassword)', CHECK_POLICY = ON;
ELSE
    ALTER LOGIN [wwsi_worker] WITH PASSWORD = '$(WorkerPassword)';
GO

USE [WwsiQueueCache];
GO
IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
        Name nvarchar(200) NOT NULL,
        Price decimal(18,2) NOT NULL,
        CreatedAtUtc datetime2 NOT NULL
    );
END;
IF OBJECT_ID(N'dbo.ProcessedCommands', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProcessedCommands
    (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_ProcessedCommands PRIMARY KEY,
        CompletedAtUtc datetime2 NOT NULL
    );
END;
GO

IF USER_ID(N'wwsi_api') IS NULL
    CREATE USER [wwsi_api] FOR LOGIN [wwsi_api];
IF USER_ID(N'wwsi_worker') IS NULL
    CREATE USER [wwsi_worker] FOR LOGIN [wwsi_worker];
GO

IF DATABASE_PRINCIPAL_ID(N'role_api_reader') IS NULL
    CREATE ROLE [role_api_reader] AUTHORIZATION [dbo];
IF DATABASE_PRINCIPAL_ID(N'role_worker_writer') IS NULL
    CREATE ROLE [role_worker_writer] AUTHORIZATION [dbo];
GO

GRANT SELECT ON OBJECT::dbo.Products TO [role_api_reader];
GRANT SELECT ON OBJECT::dbo.ProcessedCommands TO [role_api_reader];

GRANT SELECT, INSERT ON OBJECT::dbo.Products TO [role_worker_writer];
GRANT SELECT, INSERT ON OBJECT::dbo.ProcessedCommands TO [role_worker_writer];
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members rm
    JOIN sys.database_principals r ON r.principal_id = rm.role_principal_id
    JOIN sys.database_principals m ON m.principal_id = rm.member_principal_id
    WHERE r.name = N'role_api_reader' AND m.name = N'wwsi_api'
)
    ALTER ROLE [role_api_reader] ADD MEMBER [wwsi_api];
IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members rm
    JOIN sys.database_principals r ON r.principal_id = rm.role_principal_id
    JOIN sys.database_principals m ON m.principal_id = rm.member_principal_id
    WHERE r.name = N'role_worker_writer' AND m.name = N'wwsi_worker'
)
    ALTER ROLE [role_worker_writer] ADD MEMBER [wwsi_worker];
GO
PRINT 'WwsiQueueCache schema and least-privilege roles ready.';
GO
