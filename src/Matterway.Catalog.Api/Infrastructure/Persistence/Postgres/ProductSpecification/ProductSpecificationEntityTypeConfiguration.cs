using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductSpecification = Matterway.Catalog.Api.Domain.Entities.ProductSpecification;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.ProductSpecification;

internal sealed class ProductSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<DomainProductSpecification>
{
    public void Configure(EntityTypeBuilder<DomainProductSpecification> builder)
    {
        builder.ToTable(nameof(DomainProductSpecification));

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

        builder.HasData(
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "power",
                Value = 24m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "width",
                Value = 84m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "height",
                Value = 84m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "depth",
                Value = 28m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "screen-size",
                Value = 2.0m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                SpecificationSlug = "battery-size",
                Value = 3000m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                SpecificationSlug = "weight",
                Value = 400m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                SpecificationSlug = "power",
                Value = 9m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                SpecificationSlug = "weight",
                Value = 72m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                SpecificationSlug = "weight",
                Value = 970m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                SpecificationSlug = "power",
                Value = 15m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                SpecificationSlug = "power",
                Value = 8m
            },
            new DomainProductSpecification
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                SpecificationSlug = "weight",
                Value = 480m
            }
        );
    }
}