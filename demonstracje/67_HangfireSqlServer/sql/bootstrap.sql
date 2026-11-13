:ON ERROR EXIT
USE [master];
GO
IF DB_ID(N'HangfireLab67') IS NULL CREATE DATABASE [HangfireLab67];
GO
IF SUSER_ID(N'demo67_hangfire') IS NULL
    CREATE LOGIN [demo67_hangfire] WITH PASSWORD=N'$(AppPassword)', CHECK_POLICY=ON;
GO
USE [HangfireLab67];
GO
IF USER_ID(N'demo67_hangfire') IS NULL CREATE USER [demo67_hangfire] FOR LOGIN [demo67_hangfire];
-- LAB ONLY: schema installation on startup; production should use separately provisioned schema.
IF IS_ROLEMEMBER(N'db_owner',N'demo67_hangfire') <> 1
    ALTER ROLE [db_owner] ADD MEMBER [demo67_hangfire];
GO
IF OBJECT_ID(N'dbo.JobExecutions','U') IS NULL
    CREATE TABLE dbo.JobExecutions
    (RunKey nvarchar(120) NOT NULL CONSTRAINT PK_JobExecutions PRIMARY KEY,
     Kind nvarchar(40) NOT NULL, ExecutedAtUtc datetime2(3) NOT NULL);
IF OBJECT_ID(N'dbo.JobAttempts','U') IS NULL
    CREATE TABLE dbo.JobAttempts
    (RunKey nvarchar(120) NOT NULL CONSTRAINT PK_JobAttempts PRIMARY KEY,
     Attempts int NOT NULL);
GO
