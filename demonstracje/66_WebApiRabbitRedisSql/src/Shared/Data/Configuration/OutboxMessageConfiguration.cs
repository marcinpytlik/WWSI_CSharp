using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueCacheDemo.Entities;

namespace QueueCacheDemo.Data.Configuration;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> b)
    {
        b.ToTable("OutboxMessages", "dbo");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Payload).IsRequired();
        b.HasIndex(x => x.PublishedAtUtc);
        b.HasIndex(x => new { x.PublishedAtUtc, x.LeaseUntilUtc, x.CreatedAtUtc }).HasDatabaseName("IX_OutboxMessages_Lease");
        b.HasIndex(x => x.OperationId).IsUnique();
    }
}
