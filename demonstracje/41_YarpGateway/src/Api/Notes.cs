using Microsoft.EntityFrameworkCore;

namespace Demo41;

public sealed class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
}

public sealed class NotesDb : DbContext
{
    public NotesDb(DbContextOptions<NotesDb> options) : base(options) { }
    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(e =>
        {
            e.ToTable("Notes");
            e.Property(n => n.Title).HasMaxLength(120).IsRequired();
        });
    }
}
