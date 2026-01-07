using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Providers.Persistence.ProductSpecificationEntity;

internal sealed class ProductSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<ProductSpecification>
{
    public void Configure(EntityTypeBuilder<ProductSpecification> builder)
    {
        builder.ToTable(nameof(ProductSpecification));

        builder.HasKey(ps => new { ps.ProductId, ps.SpecificationSlug });

        builder.Property(ps => ps.SpecificationSlug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(ps => ps.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(ps => ps.Specification)
            .WithMany()
            .HasPrincipalKey(s => s.Slug)
            .HasForeignKey(ps => ps.SpecificationSlug);

        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSpecifications)
            .HasForeignKey(ps => ps.ProductId);
    }
}