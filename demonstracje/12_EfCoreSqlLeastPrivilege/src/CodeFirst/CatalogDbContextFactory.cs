using Demo12;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Demo12.CodeFirst;

/// <summary>
/// Fabryka dla <c>dotnet ef migrations add</c> — zawsze konto <c>demo12_deploy</c>.
/// Aplikacja nie używa tej klasy w runtime.
/// </summary>
public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlServer(SqlAccounts.CodeFirstDeploy)
            .Options;
        return new CatalogDbContext(options);
    }
}
