using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductDetailType = Matterway.Catalog.Api.Domain.Entities.ProductDetailType;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;

internal sealed class ProductDetailTypeEntityTypeConfiguration : IEntityTypeConfiguration<DomainProductDetailType>
{
    public void Configure(EntityTypeBuilder<DomainProductDetailType> builder)
    {
        builder.ToTable(nameof(DomainProductDetailType));
        builder.HasKey(pdt => pdt.Slug);
        builder.Property(pdt => pdt.Slug)
            .HasMaxLength(80)
            .IsRequired();
        builder.Property(pdt => pdt.Title)
            .HasMaxLength(80)
            .IsRequired();
        builder.Property(pdt => pdt.Unit)
            .HasMaxLength(20);

        builder.HasData(
            new DomainProductDetailType
            {
                Slug = "width",
                Title = "Width",
                Unit = "millimeters"
            },
            new DomainProductDetailType
            {
                Slug = "height",
                Title = "Height",
                Unit = "millimeters"
            },
            new DomainProductDetailType
            {
                Slug = "depth",
                Title = "Depth",
                Unit = "millimeters"
            },
            new DomainProductDetailType
            {
                Slug = "weight",
                Title = "Weight",
                Unit = "grams"
            },
            new DomainProductDetailType
            {
                Slug = "color",
                Title = "Color",
                Unit = null
            },
            new DomainProductDetailType
            {
                Slug = "material",
                Title = "Material",
                Unit = null
            },
            new DomainProductDetailType
            {
                Slug = "compatibility",
                Title = "Compatibility",
                Unit = null
            },
            new DomainProductDetailType
            {
                Slug = "power",
                Title = "Power",
                Unit = "Watts"
            },
            new DomainProductDetailType
            {
                Slug = "battery",
                Title = "Battery",
                Unit = null
            },
            new DomainProductDetailType
            {
                Slug = "connectivity",
                Title = "Connectivity",
                Unit = null
            },
            new DomainProductDetailType
            {
                Slug = "display",
                Title = "Display",
                Unit = null
            },
            new DomainProductDetailType
            {
                Slug = "storage",
                Title = "Storage",
                Unit = "GB"
            },
            new DomainProductDetailType
            {
                Slug = "color-temperature",
                Title = "Color Temperature",
                Unit = "Kelvin"
            },
            new DomainProductDetailType
            {
                Slug = "video-quality",
                Title = "Video Quality",
                Unit = null
            },
            new DomainProductDetailType
            {
                Slug = "audio-quality",
                Title = "Audio Quality",
                Unit = null
            }
        );
    }
}