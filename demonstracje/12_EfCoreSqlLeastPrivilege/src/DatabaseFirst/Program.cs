using Demo12;
using Demo12.DatabaseFirst;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var command = args.Length > 0 ? args[0] : "help";
var tryDdl = args.Contains("--try-ddl", StringComparer.OrdinalIgnoreCase);

var builder = Host.CreateApplicationBuilder(args);
var appCs = builder.Configuration.GetConnectionString("App") ?? SqlAccounts.DatabaseFirstApp;
var deployCs = builder.Configuration.GetConnectionString("Deploy") ?? SqlAccounts.DatabaseFirstDeploy;

if (!SqlAccounts.IsAppConnection(appCs) || !SqlAccounts.IsDeployConnection(deployCs))
{
    Console.Error.WriteLine("appsettings.json: Deploy musi używać demo12_deploy, App — demo12_app.");
    return 1;
}

return command switch
{
    "app" when tryDdl => await RunAppTryDdlAsync(appCs),
    "app" => await RunAppAsync(appCs, args),
    "deploy-info" => PrintDeployInfo(deployCs),
    _ => PrintHelp()
};

static int PrintHelp()
{
    Console.WriteLine("""
        Database First — źródłem prawdy jest SQL (sql/01_dbfirst_schema.sql), nie migracje C#.
        Skrypt schematu wykonuje konto demo12_deploy. Aplikacja używa demo12_app.

          app                 CRUD jako demo12_app
          app --try-ddl       pokaż, że aplikacja nie może CREATE TABLE
          deploy-info         przypomnienie: schemat wdraża SQL, nie EnsureCreated/Migrate

        Przykład:
          dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/DatabaseFirst -- app SKU-7 "Długopis" 3.20
        """);
    return 0;
}

static int PrintDeployInfo(string deployCs)
{
    Console.WriteLine("Database First: schemat wdraża sql/01_dbfirst_schema.sql jako demo12_deploy.");
    Console.WriteLine("Nie wołamy Database.Migrate() ani EnsureCreated() w tej aplikacji.");
    Console.WriteLine($"Konto wdrożeniowe (nie używane przez app): User Id={SqlAccounts.DeployUser}");
    Console.WriteLine(SqlAccounts.IsDeployConnection(deployCs)
        ? "Connection string Deploy wskazuje konto wdrożeniowe."
        : "Błąd: Deploy nie wskazuje demo12_deploy.");
    return 0;
}

static async Task<int> RunAppAsync(string connectionString, string[] args)
{
    await using var db = CreateDb(connectionString);
    await SqlRetry.WaitAsync(async () =>
    {
        if (!await db.Database.CanConnectAsync())
            throw new InvalidOperationException("Cannot connect to SQL Server.");
    }, Console.Out);
    var svc = new ProductService(new EfProductStore(db));
    if (args.Length >= 4)
        await svc.AddAsync(args[1], args[2], decimal.Parse(args[3], System.Globalization.CultureInfo.InvariantCulture));
    Console.WriteLine("Aplikacja działa jako demo12_app (schemat powstał z SQL, nie z migracji).");
    foreach (var p in await svc.ListAsync())
        Console.WriteLine($"{p.Id}\t{p.Sku}\t{p.Name}\t{p.Price:0.00}");
    return 0;
}

static async Task<int> RunAppTryDdlAsync(string connectionString)
{
    await using var db = CreateDb(connectionString);
    try
    {
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE dbo.Demo12_AppShouldNotCreate (Id INT);");
        Console.Error.WriteLine("Nieoczekiwane: konto aplikacji utworzyło tabelę — sprawdź DENY CREATE TABLE.");
        return 2;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Oczekiwany błąd: demo12_app nie może wykonywać DDL.");
        Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
        return 0;
    }
}

static CatalogDbContext CreateDb(string connectionString)
    => new(new DbContextOptionsBuilder<CatalogDbContext>().UseSqlServer(connectionString).Options);
