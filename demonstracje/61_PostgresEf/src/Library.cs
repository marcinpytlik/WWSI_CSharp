using Microsoft.EntityFrameworkCore;

namespace Demo61;

public sealed class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Year { get; set; }
}

public sealed class LibraryDb : DbContext
{
    public LibraryDb(DbContextOptions<LibraryDb> options) : base(options) { }
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(e =>
        {
            e.ToTable("Books");
            e.Property(b => b.Title).HasMaxLength(200).IsRequired();
        });
    }
}
