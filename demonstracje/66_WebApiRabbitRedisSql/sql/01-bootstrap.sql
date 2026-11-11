:ON ERROR EXIT
USE [master];
GO
IF DB_ID(N'WwsiQueueCache') IS NULL CREATE DATABASE [WwsiQueueCache];
GO
-- Provisioning is separate from application startup. Existing logins are NOT re-passworded.
IF SUSER_ID(N'wwsi_api') IS NULL
    CREATE LOGIN [wwsi_api] WITH PASSWORD = '$(ApiPassword)', CHECK_POLICY = ON;
IF SUSER_ID(N'wwsi_worker') IS NULL
    CREATE LOGIN [wwsi_worker] WITH PASSWORD = '$(WorkerPassword)', CHECK_POLICY = ON;
GO
USE [WwsiQueueCache];
GO
IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
    CREATE TABLE dbo.Products (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
        PrincipalId nvarchar(128) NOT NULL CONSTRAINT DF_Products_PrincipalId DEFAULT N'legacy',
        Name nvarchar(200) NOT NULL, Price decimal(18,2) NOT NULL,
        CreatedAtUtc datetime2 NOT NULL);
IF COL_LENGTH(N'dbo.Products', N'PrincipalId') IS NULL
    ALTER TABLE dbo.Products ADD PrincipalId nvarchar(128) NOT NULL
        CONSTRAINT DF_Products_PrincipalId DEFAULT N'legacy';
IF OBJECT_ID(N'dbo.ProcessedCommands', N'U') IS NULL
    CREATE TABLE dbo.ProcessedCommands (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_ProcessedCommands PRIMARY KEY,
        CompletedAtUtc datetime2 NOT NULL);
GO
IF OBJECT_ID(N'dbo.OperationStatuses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OperationStatuses (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_OperationStatuses PRIMARY KEY,
        ProductId uniqueidentifier NOT NULL,
        PrincipalId nvarchar(128) NOT NULL,
        IdempotencyKey nvarchar(128) NOT NULL,
        RequestHash varchar(64) NOT NULL,
        Status nvarchar(24) NOT NULL,
        CreatedAtUtc datetime2 NOT NULL,
        UpdatedAtUtc datetime2 NOT NULL,
        ErrorCode nvarchar(120) NULL);
    CREATE UNIQUE INDEX UX_OperationStatuses_Principal_Key
        ON dbo.OperationStatuses(PrincipalId, IdempotencyKey);
END;
IF OBJECT_ID(N'dbo.OutboxMessages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OutboxMessages (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_OutboxMessages PRIMARY KEY,
        OperationId uniqueidentifier NOT NULL,
        Payload nvarchar(max) NOT NULL,
        CreatedAtUtc datetime2 NOT NULL,
        PublishedAtUtc datetime2 NULL,
        LeaseOwner uniqueidentifier NULL,
        LeaseUntilUtc datetime2 NULL);
    CREATE UNIQUE INDEX UX_OutboxMessages_OperationId ON dbo.OutboxMessages(OperationId);
    CREATE INDEX IX_OutboxMessages_PublishedAtUtc ON dbo.OutboxMessages(PublishedAtUtc);
END;
IF COL_LENGTH(N'dbo.OutboxMessages', N'LeaseOwner') IS NULL
    ALTER TABLE dbo.OutboxMessages ADD LeaseOwner uniqueidentifier NULL;
IF COL_LENGTH(N'dbo.OutboxMessages', N'LeaseUntilUtc') IS NULL
    ALTER TABLE dbo.OutboxMessages ADD LeaseUntilUtc datetime2 NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.OutboxMessages') AND name=N'IX_OutboxMessages_Lease')
    CREATE INDEX IX_OutboxMessages_Lease
        ON dbo.OutboxMessages(PublishedAtUtc, LeaseUntilUtc, CreatedAtUtc);
GO
IF USER_ID(N'wwsi_api') IS NULL CREATE USER [wwsi_api] FOR LOGIN [wwsi_api];
IF USER_ID(N'wwsi_worker') IS NULL CREATE USER [wwsi_worker] FOR LOGIN [wwsi_worker];
GO
IF DATABASE_PRINCIPAL_ID(N'role_api_reader') IS NULL
    CREATE ROLE [role_api_reader] AUTHORIZATION [dbo];
IF DATABASE_PRINCIPAL_ID(N'role_worker_writer') IS NULL
    CREATE ROLE [role_worker_writer] AUTHORIZATION [dbo];
GO
GRANT SELECT ON OBJECT::dbo.Products TO [role_api_reader];
REVOKE SELECT ON OBJECT::dbo.ProcessedCommands FROM [role_api_reader];
GRANT SELECT ON OBJECT::dbo.OperationStatuses TO [role_api_reader];
GRANT INSERT ON OBJECT::dbo.OperationStatuses TO [role_api_reader];
GRANT INSERT ON OBJECT::dbo.OutboxMessages TO [role_api_reader];

GRANT SELECT, INSERT ON OBJECT::dbo.Products TO [role_worker_writer];
GRANT SELECT, INSERT ON OBJECT::dbo.ProcessedCommands TO [role_worker_writer];
GRANT SELECT, UPDATE ON OBJECT::dbo.OperationStatuses TO [role_worker_writer];
GRANT SELECT, UPDATE ON OBJECT::dbo.OutboxMessages TO [role_worker_writer];
GO
IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members rm
    JOIN sys.database_principals r ON r.principal_id=rm.role_principal_id
    JOIN sys.database_principals m ON m.principal_id=rm.member_principal_id
    WHERE r.name=N'role_api_reader' AND m.name=N'wwsi_api')
    ALTER ROLE [role_api_reader] ADD MEMBER [wwsi_api];
IF NOT EXISTS (
    SELECT 1 FROM sys.database_role_members rm
    JOIN sys.database_principals r ON r.principal_id=rm.role_principal_id
    JOIN sys.database_principals m ON m.principal_id=rm.member_principal_id
    WHERE r.name=N'role_worker_writer' AND m.name=N'wwsi_worker')
    ALTER ROLE [role_worker_writer] ADD MEMBER [wwsi_worker];
GO
IF OBJECT_ID(N'dbo.SchemaVersions', N'U') IS NULL
    CREATE TABLE dbo.SchemaVersions (VersionKey nvarchar(64) NOT NULL PRIMARY KEY, AppliedAtUtc datetime2 NOT NULL);
IF NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE VersionKey = N'2026-11-11-reference-v2')
    INSERT INTO dbo.SchemaVersions(VersionKey, AppliedAtUtc) VALUES(N'2026-11-11-reference-v2', SYSUTCDATETIME());
PRINT 'Provisioned WwsiQueueCache; existing data and logins retained.';
GO
