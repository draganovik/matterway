using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Providers.Persistence.ProductDetailEntity;

internal sealed class ProductDetailEntityTypeConfiguration : IEntityTypeConfiguration<ProductDetail>
{
    public void Configure(EntityTypeBuilder<ProductDetail> builder)
    {
        builder.ToTable(nameof(ProductDetail));

        builder.HasKey(pd => new { pd.ProductId, pd.DetailSlug });

        builder.Property(pd => pd.DetailSlug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(pd => pd.Value)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(pd => pd.Detail)
            .WithMany()
            .HasPrincipalKey(d => d.Slug)
            .HasForeignKey(pd => pd.DetailSlug);

        builder.HasOne(pd => pd.Product)
            .WithMany(p => p.ProductDetails)
            .HasForeignKey(pd => pd.ProductId);
    }
}