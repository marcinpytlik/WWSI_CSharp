using Demo58;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Demo58.Tests;

public class TrackingTests
{
    [Fact]
    public async Task Tracked_SaveChanges_Persists()
    {
        var (options, connection) = await OpenAsync();
        await using var _ = connection;
        await using var db = new NotesDb(options);
        await NoteEdits.MutateTrackedAsync(db, 1, "Tracked");
        Assert.Equal("Tracked", (await db.Notes.AsNoTracking().SingleAsync()).Title);
    }

    [Fact]
    public async Task AsNoTracking_SaveChanges_DoesNotPersist()
    {
        var (options, connection) = await OpenAsync();
        await using var _ = connection;
        await using var db = new NotesDb(options);
        await NoteEdits.MutateNoTrackingAsync(db, 1, "Ghost");
        Assert.Equal("Original", (await db.Notes.AsNoTracking().SingleAsync()).Title);
    }

    private static async Task<(DbContextOptions<NotesDb> Options, SqliteConnection Connection)> OpenAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<NotesDb>().UseSqlite(connection).Options;
        await using (var setup = new NotesDb(options))
        {
            await setup.Database.EnsureCreatedAsync();
            setup.Notes.Add(new Note { Title = "Original" });
            await setup.SaveChangesAsync();
        }

        return (options, connection);
    }
}
