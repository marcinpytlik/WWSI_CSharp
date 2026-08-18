using Microsoft.EntityFrameworkCore;

namespace Demo21;

public sealed class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Book> Books { get; set; } = [];
}

public sealed class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}

public sealed class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author!)
            .HasForeignKey(b => b.AuthorId);
        modelBuilder.Entity<Author>().Property(a => a.Name).HasMaxLength(80).IsRequired();
        modelBuilder.Entity<Book>().Property(b => b.Title).HasMaxLength(200).IsRequired();
    }
}

public sealed class LibraryService
{
    private readonly LibraryContext _db;
    public LibraryService(LibraryContext db) => _db = db;

    public async Task<Author> AddAuthorWithBookAsync(string author, string title)
    {
        var entity = new Author { Name = author, Books = [new Book { Title = title }] };
        _db.Authors.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public Task<List<Author>> AuthorsWithBooksAsync()
        => _db.Authors.Include(a => a.Books).OrderBy(a => a.Name).ToListAsync();
}

public static class Program
{
    public static async Task<int> Main()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>().UseSqlite("Data Source=demo21.db").Options;
        await using var db = new LibraryContext(options);
        await db.Database.EnsureCreatedAsync();
        var svc = new LibraryService(db);
        await svc.AddAuthorWithBookAsync("Fowler", "Refactoring");
        foreach (var a in await svc.AuthorsWithBooksAsync())
            Console.WriteLine($"{a.Name}: {string.Join(", ", a.Books.Select(b => b.Title))}");
        return 0;
    }
}
