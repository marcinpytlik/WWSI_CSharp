using Demo12;
using Demo12.CodeFirst;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var command = args.Length > 0 ? args[0] : "help";
var tryMigrate = args.Contains("--try-migrate", StringComparer.OrdinalIgnoreCase);

var builder = Host.CreateApplicationBuilder(args);
var deployCs = builder.Configuration.GetConnectionString("Deploy") ?? SqlAccounts.CodeFirstDeploy;
var appCs = builder.Configuration.GetConnectionString("App") ?? SqlAccounts.CodeFirstApp;

if (!SqlAccounts.IsDeployConnection(deployCs) || !SqlAccounts.IsAppConnection(appCs))
{
    Console.Error.WriteLine("appsettings.json: Deploy musi używać demo12_deploy, App — demo12_app.");
    return 1;
}

return command switch
{
    "deploy" => await RunDeployAsync(deployCs),
    "app" when tryMigrate => await RunAppTryMigrateAsync(appCs),
    "app" => await RunAppAsync(appCs, args),
    _ => PrintHelp()
};

static int PrintHelp()
{
    Console.WriteLine("""
        Code First — schemat z C# / migracji EF. Dwa konta SQL:

          deploy          konto demo12_deploy: Database.Migrate() + seed
          app             konto demo12_app: CRUD (bez DDL)
          app --try-migrate   pokaż, że aplikacja NIE może wdrażać schematu

        Przykłady:
          dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/CodeFirst -- deploy
          dotnet run --project demonstracje/12_EfCoreSqlLeastPrivilege/src/CodeFirst -- app SKU-42 "Notes" 12.50
        """);
    return 0;
}

static async Task<int> RunDeployAsync(string connectionString)
{
    await using var db = CreateDb(connectionString);
    await SqlRetry.WaitAsync(() => db.Database.MigrateAsync(), Console.Out);
    var svc = new ProductService(new EfProductStore(db));
    if (!(await svc.ListAsync()).Any())
        await svc.AddAsync("SKU-1", "Notes A6", 9.90m);
    Console.WriteLine("Wdrożono schemat (Migrate) jako demo12_deploy.");
    await PrintProductsAsync(svc);
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
    Console.WriteLine("Aplikacja działa jako demo12_app (tylko DML).");
    await PrintProductsAsync(svc);
    return 0;
}

static async Task<int> RunAppTryMigrateAsync(string connectionString)
{
    await using var db = CreateDb(connectionString);
    try
    {
        await db.Database.MigrateAsync();
        Console.WriteLine("Migrate() nic nie zmieniło albo było już zastosowane — sprawdzam CREATE TABLE…");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migrate() odrzucone: demo12_app nie wdraża schematu.");
        Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
    }

    try
    {
        await db.Database.ExecuteSqlRawAsync("CREATE TABLE dbo.Demo12_AppShouldNotCreate (Id INT);");
        Console.Error.WriteLine("Nieoczekiwane: konto aplikacji utworzyło tabelę — sprawdź DENY CREATE TABLE.");
        return 2;
    }
    catch (Exception ex)
    {
        Console.WriteLine("CREATE TABLE odrzucone dla demo12_app (zasada minimalnych uprawnień).");
        Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
        return 0;
    }
}

static CatalogDbContext CreateDb(string connectionString)
    => new(new DbContextOptionsBuilder<CatalogDbContext>().UseSqlServer(connectionString).Options);

static async Task PrintProductsAsync(ProductService svc)
{
    foreach (var p in await svc.ListAsync())
        Console.WriteLine($"{p.Id}\t{p.Sku}\t{p.Name}\t{p.Price:0.00}");
}
