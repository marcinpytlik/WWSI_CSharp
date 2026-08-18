using Microsoft.EntityFrameworkCore;

namespace Demo58;

public sealed class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
}

public sealed class NotesDb : DbContext
{
    public NotesDb(DbContextOptions<NotesDb> options) : base(options) { }
    public DbSet<Note> Notes => Set<Note>();
}

public static class NoteEdits
{
    public static async Task MutateTrackedAsync(NotesDb db, int id, string title)
    {
        var note = await db.Notes.FirstAsync(n => n.Id == id);
        note.Title = title;
        await db.SaveChangesAsync();
    }

    public static async Task MutateNoTrackingAsync(NotesDb db, int id, string title)
    {
        var note = await db.Notes.AsNoTracking().FirstAsync(n => n.Id == id);
        note.Title = title;
        await db.SaveChangesAsync();
    }
}

public static class Program
{
    public static async Task<int> Main()
    {
        var options = new DbContextOptionsBuilder<NotesDb>().UseSqlite("Data Source=demo58.db").Options;
        await using var db = new NotesDb(options);
        await db.Database.EnsureCreatedAsync();
        Console.WriteLine("AsNoTracking vs tracking — see tests.");
        return 0;
    }
}
