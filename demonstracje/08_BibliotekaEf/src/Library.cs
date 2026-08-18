using Microsoft.EntityFrameworkCore;

namespace Demo08;

public sealed class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Year { get; set; }
}

public sealed class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
}

public sealed class BookService
{
    private readonly LibraryContext _db;

    public BookService(LibraryContext db) => _db = db;

    public async Task<Book> AddAsync(string title, int year)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (year < 1)
            throw new ArgumentOutOfRangeException(nameof(year));

        var book = new Book { Title = title.Trim(), Year = year };
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public Task<List<Book>> AllAsync() => _db.Books.OrderBy(b => b.Title).ToListAsync();
}

public static class Program
{
    public static async Task<int> Main()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlite("Data Source=demo08.db")
            .Options;
        await using var db = new LibraryContext(options);
        await db.Database.EnsureCreatedAsync();
        var svc = new BookService(db);
        await svc.AddAsync("Clean Code", 2008);
        foreach (var book in await svc.AllAsync())
            Console.WriteLine($"{book.Id}. {book.Title} ({book.Year})");
        return 0;
    }
}
