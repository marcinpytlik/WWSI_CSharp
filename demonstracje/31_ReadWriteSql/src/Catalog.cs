using Microsoft.EntityFrameworkCore;

namespace Demo31;

public sealed class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
}

public abstract class CatalogDb : DbContext
{
    protected CatalogDb(DbContextOptions options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(x => x.Sku).IsUnique();
            e.Property(x => x.Sku).HasMaxLength(32).IsRequired();
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
        });
    }
}

public sealed class WriteCatalogDb : CatalogDb
{
    public WriteCatalogDb(DbContextOptions<WriteCatalogDb> options) : base(options) { }
}

public sealed class ReadCatalogDb : CatalogDb
{
    public ReadCatalogDb(DbContextOptions<ReadCatalogDb> options) : base(options) { }
}

public static class SqlAccounts
{
    public const string WriteUser = "demo31_write";
    public const string ReadUser = "demo31_read";
}

public sealed class CatalogWriter
{
    private readonly WriteCatalogDb _db;
    public CatalogWriter(WriteCatalogDb db) => _db = db;

    public async Task<Product> AddAsync(string sku, string name)
    {
        var product = new Product { Sku = sku.Trim().ToUpperInvariant(), Name = name.Trim() };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }
}

public sealed class CatalogReader
{
    private readonly ReadCatalogDb _db;
    public CatalogReader(ReadCatalogDb db) => _db = db;

    public Task<List<Product>> ListAsync()
        => _db.Products.AsNoTracking().OrderBy(p => p.Sku).ToListAsync();
}

public static class Program
{
    public static async Task<int> Main()
    {
        var cs = "Data Source=demo31.db";
        var writeOpt = new DbContextOptionsBuilder<WriteCatalogDb>().UseSqlite(cs).Options;
        var readOpt = new DbContextOptionsBuilder<ReadCatalogDb>().UseSqlite(cs).Options;
        await using var write = new WriteCatalogDb(writeOpt);
        await write.Database.EnsureCreatedAsync();
        await using var read = new ReadCatalogDb(readOpt);
        await new CatalogWriter(write).AddAsync("SKU-1", "Notes");
        foreach (var p in await new CatalogReader(read).ListAsync())
            Console.WriteLine($"{p.Sku} {p.Name}");
        Console.WriteLine($"write login={SqlAccounts.WriteUser} read login={SqlAccounts.ReadUser}");
        return 0;
    }
}
