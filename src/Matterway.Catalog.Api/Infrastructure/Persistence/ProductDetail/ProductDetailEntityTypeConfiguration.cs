using Matterway.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductDetail = Matterway.Catalog.Api.Domain.ProductDetail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;

internal sealed class ProductDetailEntityTypeConfiguration : IEntityTypeConfiguration<DomainProductDetail>
{
    public void Configure(EntityTypeBuilder<DomainProductDetail> builder)
    {
        builder.ToTable(nameof(DomainProductDetail));

        builder.HasKey(pd => pd.Id);

        builder.Property(pd => pd.ProductId)
            .IsRequired();

        builder.Property(pd => pd.Type)
            .IsRequired();

        builder.Property(pd => pd.Title)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(pd => pd.Value)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pd => pd.Unit)
            .HasMaxLength(20);

        builder.HasOne(pd => pd.Product)
            .WithMany(p => p.ProductDetails!)
            .HasForeignKey(pd => pd.ProductId);

        builder.HasData(
            new DomainProductDetail
            {
                Id = Guid.Parse("fd6f8de6-91c6-4362-ae90-6d8cf1d98f27"),
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Type = DetailType.Specification,
                Title = "Compatibility",
                Value = "Works with Alexa, Google Assistant, and Apple HomeKit"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("30ef1d9a-13f8-4c2a-a2c3-5e5a5c23f5e1"),
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Type = DetailType.Specification,
                Title = "Display",
                Value = "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("b15e8e32-f357-4f97-9d19-1d2668e6d31a"),
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Type = DetailType.Specification,
                Title = "Power",
                Value = "Requires 24VAC power, uses less than 1 kWh/month"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("eb69b58c-8f2d-48ee-b3eb-49a9d0ce49cb"),
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Type = DetailType.Specification,
                Title = "Connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("ab0e76b4-69ea-4cc4-8cc2-f1d672dc2e2d"),
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Type = DetailType.Specification,
                Title = "Compatibility",
                Value = "Works with Alexa, Google Assistant, and Siri"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("0b80edc9-5351-4d75-9c12-7f53c15e74b8"),
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Type = DetailType.Specification,
                Title = "Battery",
                Value = "Uses four AA batteries (included), lasts up to 6 months depending on usage"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("3692d929-1534-4d4d-aae9-ec9e757b77c5"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Type = DetailType.Specification,
                Title = "Color Temperature",
                Value = "Adjustable from warm white (2700K) to daylight (6500K)"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("a46a6ea7-1e2c-427d-91f8-3d020b34d09c"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Type = DetailType.Specification,
                Title = "Compatibility",
                Value = "Works with Alexa, Google Assistant, and Samsung SmartThings"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("f5e5f5c5-5bf5-4c20-8b2d-f2f719e78508"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Type = DetailType.Specification,
                Title = "Power",
                Value = "9",
                Unit = "Watt"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("5b5eaa60-3fb6-44f6-8640-bc56a55c986f"),
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Type = DetailType.Specification,
                Title = "Weight",
                Value = "970",
                Unit = "gram"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("ae18a00e-7f3b-4df3-8d4c-df4a0b271a87"),
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Type = DetailType.Specification,
                Title = "Connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("f69c6d88-3a1c-46e8-bbcf-16d274f63052"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Type = DetailType.Specification,
                Title = "Video",
                Value = "1080p HD"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("0a108c0c-d5b5-4486-90a6-0e7eb8d25a3c"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Type = DetailType.Specification,
                Title = "Audio",
                Value = "Two-way audio with noise cancellation"
            },
            new DomainProductDetail
            {
                Id = Guid.Parse("259bdf85-efb1-42e7-a8d1-9c7a6b71979a"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Type = DetailType.Specification,
                Title = "Connectivity",
                Value = "Wi-Fi and Ethernet"
            }
        );
    }
}