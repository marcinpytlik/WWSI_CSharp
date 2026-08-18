using Microsoft.EntityFrameworkCore;

namespace Demo42;

public sealed class EventRow
{
    public int Id { get; set; }
    public string Message { get; set; } = "";
    public string CorrelationId { get; set; } = "";
}

public sealed class EventsDb : DbContext
{
    public EventsDb(DbContextOptions<EventsDb> options) : base(options) { }
    public DbSet<EventRow> Events => Set<EventRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventRow>(e =>
        {
            e.ToTable("Events");
            e.Property(x => x.Message).HasMaxLength(200).IsRequired();
            e.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();
        });
    }
}
