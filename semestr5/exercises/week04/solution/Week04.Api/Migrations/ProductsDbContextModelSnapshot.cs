using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Week04.Api.Migrations;

[DbContext(typeof(ProductsDbContext))]
public partial class ProductsDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");

        modelBuilder.Entity("Product", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("INTEGER");
            b.Property<string>("Sku").IsRequired().HasMaxLength(32).HasColumnType("TEXT");
            b.Property<string>("Name").IsRequired().HasMaxLength(200).HasColumnType("TEXT");
            b.Property<decimal>("Price").HasPrecision(18, 2).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("Sku").IsUnique();
            b.ToTable("Products");
        });
    }
}
