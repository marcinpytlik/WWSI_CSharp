-- Schemat Database First — źródło prawdy. Uruchamiać jako demo12_deploy (nie jako sa, nie jako app).
USE Demo12_DbFirst;
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        Id INT IDENTITY(1, 1) NOT NULL
            CONSTRAINT PK_Products PRIMARY KEY,
        Sku NVARCHAR(32) NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Price DECIMAL(18, 2) NOT NULL,
        CONSTRAINT UQ_Products_Sku UNIQUE (Sku)
    );
END
GO
