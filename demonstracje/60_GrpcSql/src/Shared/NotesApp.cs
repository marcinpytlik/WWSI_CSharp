using Microsoft.EntityFrameworkCore;

namespace Demo60;

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

public sealed class NotesApp
{
    private readonly NotesDb _db;
    public NotesApp(NotesDb db) => _db = db;

    public async Task<Note> AddAsync(string title, CancellationToken cancellationToken)
    {
        var note = new Note { Title = title.Trim() };
        _db.Notes.Add(note);
        await _db.SaveChangesAsync(cancellationToken);
        return note;
    }

    public Task<List<Note>> ListAsync(CancellationToken cancellationToken)
        => _db.Notes.AsNoTracking().OrderBy(n => n.Id).ToListAsync(cancellationToken);
}
