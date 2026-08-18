using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Demo43;

public sealed class ChatMessage
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public DateTime SentUtc { get; set; }
}

public sealed class ChatDb : DbContext
{
    public ChatDb(DbContextOptions<ChatDb> options) : base(options) { }
    public DbSet<ChatMessage> Messages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>(e =>
        {
            e.ToTable("Messages");
            e.Property(m => m.Text).HasMaxLength(400).IsRequired();
        });
    }
}

public sealed class ChatHub : Hub
{
    private readonly ChatDb _db;
    public ChatHub(ChatDb db) => _db = db;

    public async Task Send(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Trim().Length < 2)
            throw new HubException("Message must have at least 2 characters.");
        var row = new ChatMessage { Text = text.Trim(), SentUtc = DateTime.UtcNow };
        _db.Messages.Add(row);
        await _db.SaveChangesAsync();
        await Clients.All.SendAsync("Receive", row.Id, row.Text, row.SentUtc);
    }
}
