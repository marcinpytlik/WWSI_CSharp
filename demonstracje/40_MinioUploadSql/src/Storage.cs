using Microsoft.EntityFrameworkCore;

namespace Demo40;

public interface IBlobStore
{
    Task<string> PutAsync(string fileName, string contentType, Stream content, CancellationToken cancellationToken);
    Task<BlobRead?> GetAsync(string key, CancellationToken cancellationToken);
}

public sealed record BlobRead(byte[] Bytes, string ContentType, string FileName);

public sealed class InMemoryBlobStore : IBlobStore
{
    private readonly Dictionary<string, BlobRead> _items = new();
    private readonly object _gate = new();

    public Task<string> PutAsync(string fileName, string contentType, Stream content, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        content.CopyTo(ms);
        var key = $"{Guid.NewGuid():N}-{fileName}";
        lock (_gate)
            _items[key] = new BlobRead(ms.ToArray(), contentType, fileName);
        return Task.FromResult(key);
    }

    public Task<BlobRead?> GetAsync(string key, CancellationToken cancellationToken)
    {
        lock (_gate)
            return Task.FromResult(_items.TryGetValue(key, out var blob) ? blob : null);
    }
}

public sealed class StoredFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public string ContentType { get; set; } = "";
    public string BlobKey { get; set; } = "";
    public long Size { get; set; }
}

public sealed class FilesDb : DbContext
{
    public FilesDb(DbContextOptions<FilesDb> options) : base(options) { }
    public DbSet<StoredFile> Files => Set<StoredFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StoredFile>(e =>
        {
            e.ToTable("Files");
            e.Property(f => f.FileName).HasMaxLength(200).IsRequired();
            e.Property(f => f.ContentType).HasMaxLength(120).IsRequired();
            e.Property(f => f.BlobKey).HasMaxLength(260).IsRequired();
        });
    }
}
