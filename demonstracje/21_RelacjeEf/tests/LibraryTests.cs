using Demo21;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Demo21.Tests;

public class LibraryTests
{
    private static async Task<LibraryContext> OpenAsync()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>().UseSqlite("DataSource=:memory:").Options;
        var db = new LibraryContext(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();
        return db;
    }

    [Fact]
    public async Task Author_HasBooks_Include()
    {
        await using var db = await OpenAsync();
        var svc = new LibraryService(db);
        await svc.AddAuthorWithBookAsync("Beck", "TDD");
        var all = await svc.AuthorsWithBooksAsync();
        Assert.Single(all);
        Assert.Equal("TDD", all[0].Books.Single().Title);
        Assert.Equal(all[0].Id, all[0].Books.Single().AuthorId);
    }

    [Fact]
    public async Task BooksWithoutInclude_HaveNoAuthor()
    {
        await using var db = await OpenAsync();
        await new LibraryService(db).AddAuthorWithBookAsync("Beck", "TDD");
        var book = await db.Books.AsNoTracking().SingleAsync();
        Assert.Null(book.Author);
    }
}
