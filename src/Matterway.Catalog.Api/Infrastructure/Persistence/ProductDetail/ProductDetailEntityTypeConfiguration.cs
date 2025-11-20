using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductDetail = Matterway.Catalog.Api.Domain.Entities.ProductDetail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;

internal sealed class ProductDetailEntityTypeConfiguration : IEntityTypeConfiguration<DomainProductDetail>
{
    public void Configure(EntityTypeBuilder<DomainProductDetail> builder)
    {
        builder.ToTable(nameof(DomainProductDetail));

        builder.HasKey(pd => new { pd.ProductId, pd.TypeSlug });

        builder.Property(pd => pd.TypeSlug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(pd => pd.Value)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(pd => pd.Type)
            .WithMany()
            .HasPrincipalKey(pdt => pdt.Slug)
            .HasForeignKey(pd => pd.TypeSlug);

        builder.HasOne(pd => pd.Product)
            .WithMany(p => p.ProductDetails)
            .HasForeignKey(pd => pd.ProductId);

        builder.HasData(
            new DomainProductDetail
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                TypeSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Apple HomeKit"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                TypeSlug = "display",
                Value = "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                TypeSlug = "power",
                Value = "Requires 24VAC power, uses less than 1 kWh/month"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                TypeSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                TypeSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Siri"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                TypeSlug = "battery",
                Value = "Uses four AA batteries (included), lasts up to 6 months depending on usage"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                TypeSlug = "color-temperature",
                Value = "Adjustable from warm white (2700K) to daylight (6500K)"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                TypeSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Samsung SmartThings"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                TypeSlug = "power",
                Value = "9"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                TypeSlug = "weight",
                Value = "970"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                TypeSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                TypeSlug = "video-quality",
                Value = "1080p HD"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                TypeSlug = "audio-quality",
                Value = "Two-way audio with noise cancellation"
            },
            new DomainProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                TypeSlug = "connectivity",
                Value = "Wi-Fi and Ethernet"
            }
        );
    }
}