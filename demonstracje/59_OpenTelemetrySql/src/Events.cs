using Microsoft.EntityFrameworkCore;

namespace Demo59;

public sealed class TraceEvent
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string TraceId { get; set; } = "";
}

public sealed class EventsDb : DbContext
{
    public EventsDb(DbContextOptions<EventsDb> options) : base(options) { }
    public DbSet<TraceEvent> Events => Set<TraceEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TraceEvent>(e =>
        {
            e.ToTable("Events");
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.TraceId).HasMaxLength(64).IsRequired();
        });
    }
}
