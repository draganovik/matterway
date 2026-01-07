using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Providers.Persistence.ProductEntity;

internal sealed class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(nameof(Product));

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.ProductCode)
            .IsUnique();

        builder.Property(p => p.ProductCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.IsAvailable)
            .IsRequired();

        builder.HasMany(p => p.ProductDetails)
            .WithOne(pd => pd.Product)
            .HasForeignKey(pd => pd.ProductId);

        builder.HasMany(p => p.ProductSpecifications)
            .WithOne(ps => ps.Product)
            .HasForeignKey(ps => ps.ProductId);

        builder.HasMany(p => p.ProductImages)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId);
    }
}