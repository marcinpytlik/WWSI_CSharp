-- Opcjonalnie po pierwszym utworzeniu tabel: zawęź DML aplikacji do tabel biznesowych.
-- Aplikacja traci dostęp do __EFMigrationsHistory (Code First) i nie dostaje nowych tabel „za darmo”.
-- Po kolejnej migracji DBA/deploy ponownie nadaje GRANT na nowe tabele.

USE Demo12_CodeFirst;
GO
REVOKE SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo FROM demo12_app;
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL
    GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::dbo.Products TO demo12_app;
GO

USE Demo12_DbFirst;
GO
REVOKE SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo FROM demo12_app;
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL
    GRANT SELECT, INSERT, UPDATE, DELETE ON OBJECT::dbo.Products TO demo12_app;
GO
