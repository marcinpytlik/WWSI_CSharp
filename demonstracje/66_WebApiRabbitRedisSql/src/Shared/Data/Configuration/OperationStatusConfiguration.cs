using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueueCacheDemo.Entities;

namespace QueueCacheDemo.Data.Configuration;

public sealed class OperationStatusConfiguration : IEntityTypeConfiguration<OperationStatus>
{
    public void Configure(EntityTypeBuilder<OperationStatus> b)
    {
        b.ToTable("OperationStatuses", "dbo");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.ProductId).IsRequired();
        b.Property(x => x.PrincipalId).HasMaxLength(128).IsRequired();
        b.Property(x => x.IdempotencyKey).HasMaxLength(128).IsRequired();
        b.Property(x => x.RequestHash).HasColumnType("varchar(64)").HasMaxLength(64).IsRequired();
        b.Property(x => x.Status).HasMaxLength(24).IsRequired();
        b.Property(x => x.ErrorCode).HasMaxLength(120);
        b.HasIndex(x => new { x.PrincipalId, x.IdempotencyKey }).IsUnique();
    }
}
