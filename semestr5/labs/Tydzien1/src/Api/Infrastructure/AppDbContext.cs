using Microsoft.EntityFrameworkCore;
using Api.Domain;

namespace Api.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Category>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Product>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).HasMaxLength(200).IsRequired();
                b.Property(x => x.Price).HasPrecision(18,2);
                b.HasOne(x => x.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
