IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'demo31_write')
    CREATE LOGIN demo31_write WITH PASSWORD = N'Demo31_Write_Pass!', CHECK_POLICY = OFF;
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'demo31_read')
    CREATE LOGIN demo31_read WITH PASSWORD = N'Demo31_Read_Pass!', CHECK_POLICY = OFF;
GO
IF DB_ID(N'Demo31Catalog') IS NULL
    CREATE DATABASE Demo31Catalog;
GO
USE Demo31Catalog;
GO
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'demo31_write')
    CREATE USER demo31_write FOR LOGIN demo31_write;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'demo31_read')
    CREATE USER demo31_read FOR LOGIN demo31_read;
GO
ALTER ROLE db_ddladmin ADD MEMBER demo31_write;
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO demo31_write;
GRANT SELECT ON SCHEMA::dbo TO demo31_read;
DENY INSERT, UPDATE, DELETE ON SCHEMA::dbo TO demo31_read;
DENY CREATE TABLE TO demo31_read;
GO
