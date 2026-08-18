using Microsoft.EntityFrameworkCore;

namespace Demo32;

public sealed class ReportsDb : DbContext
{
    public ReportsDb(DbContextOptions<ReportsDb> options) : base(options) { }

    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>(e =>
        {
            e.ToTable("Reports");
            e.Property(x => x.Title).HasMaxLength(120).IsRequired();
            e.Property(x => x.Status).HasMaxLength(32).IsRequired();
        });
    }
}
