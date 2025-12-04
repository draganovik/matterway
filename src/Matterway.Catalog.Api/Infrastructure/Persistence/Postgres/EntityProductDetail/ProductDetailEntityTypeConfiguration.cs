using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductDetail;

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

        builder.HasData(
            new ProductDetail
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Apple HomeKit"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "display",
                Value = "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Siri"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "battery",
                Value = "Uses four AA batteries (included), lasts up to 6 months depending on usage"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "color-temperature",
                Value = "Adjustable from warm white (2700K) to daylight (6500K)"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Samsung SmartThings"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "video-quality",
                Value = "1080p HD"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "audio-quality",
                Value = "Two-way audio with noise cancellation"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Ethernet"
            }
        );
    }
}