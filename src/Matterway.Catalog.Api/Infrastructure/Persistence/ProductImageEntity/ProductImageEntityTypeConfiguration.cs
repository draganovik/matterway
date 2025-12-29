using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductImageEntity;

internal sealed class ProductImageEntityTypeConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable(nameof(ProductImage));

        builder.HasKey(pi => new { pi.Id, pi.ProductId });

        builder.Property(pi => pi.OrderIndex)
            .IsRequired();

        builder.Property(pi => pi.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pi => pi.ImageAlt)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(pi => new { pi.ProductId, pi.OrderIndex }).IsUnique();

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages!)
            .HasForeignKey(pi => pi.ProductId);
    }
}