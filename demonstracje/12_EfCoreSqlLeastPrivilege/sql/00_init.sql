-- Bootstrap jako sa (DBA). Nie używać sa w aplikacji ani w migracjach EF.
-- Idempotentne: można uruchomić wielokrotnie.

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'demo12_deploy')
    CREATE LOGIN demo12_deploy WITH PASSWORD = N'Demo12_Deploy_Pass!', CHECK_POLICY = OFF;
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'demo12_app')
    CREATE LOGIN demo12_app WITH PASSWORD = N'Demo12_App_Pass!', CHECK_POLICY = OFF;
GO

IF DB_ID(N'Demo12_CodeFirst') IS NULL
    CREATE DATABASE Demo12_CodeFirst;
IF DB_ID(N'Demo12_DbFirst') IS NULL
    CREATE DATABASE Demo12_DbFirst;
GO

-- Code First ---------------------------------------------------------------
USE Demo12_CodeFirst;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'demo12_deploy')
    CREATE USER demo12_deploy FOR LOGIN demo12_deploy;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'demo12_app')
    CREATE USER demo12_app FOR LOGIN demo12_app;
GO

ALTER ROLE db_ddladmin ADD MEMBER demo12_deploy;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO demo12_deploy;
GRANT ALTER ON SCHEMA::dbo TO demo12_deploy;
GRANT CREATE TABLE TO demo12_deploy;
GRANT VIEW DEFINITION TO demo12_deploy;

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO demo12_app;
DENY ALTER ON SCHEMA::dbo TO demo12_app;
DENY CONTROL ON SCHEMA::dbo TO demo12_app;
DENY CREATE TABLE TO demo12_app;
DENY ALTER ANY ROLE TO demo12_app;
DENY ALTER ANY USER TO demo12_app;
GO

-- Database First -----------------------------------------------------------
USE Demo12_DbFirst;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'demo12_deploy')
    CREATE USER demo12_deploy FOR LOGIN demo12_deploy;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'demo12_app')
    CREATE USER demo12_app FOR LOGIN demo12_app;
GO

ALTER ROLE db_ddladmin ADD MEMBER demo12_deploy;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO demo12_deploy;
GRANT ALTER ON SCHEMA::dbo TO demo12_deploy;
GRANT CREATE TABLE TO demo12_deploy;
GRANT VIEW DEFINITION TO demo12_deploy;

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO demo12_app;
DENY ALTER ON SCHEMA::dbo TO demo12_app;
DENY CONTROL ON SCHEMA::dbo TO demo12_app;
DENY CREATE TABLE TO demo12_app;
DENY ALTER ANY ROLE TO demo12_app;
DENY ALTER ANY USER TO demo12_app;
GO
