using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductDetailType = Matterway.Catalog.Api.Domain.ProductDetailType;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;

internal sealed class ProductDetailTypeEntityTypeConfiguration : IEntityTypeConfiguration<DomainProductDetailType>
{
    public void Configure(EntityTypeBuilder<DomainProductDetailType> builder)
    {
        builder.ToTable(nameof(DomainProductDetailType));
        builder.HasKey(pdt => pdt.Id);
        builder.Property(pdt => pdt.Title)
            .HasMaxLength(80)
            .IsRequired();
        builder.Property(pdt => pdt.Unit)
            .HasMaxLength(20);

        builder.HasData(
            new DomainProductDetailType
            {
                Id = 1,
                Title = "Width",
                Unit = "millimeters"
            },
            new DomainProductDetailType
            {
                Id = 2,
                Title = "Height",
                Unit = "millimeters"
            },
            new DomainProductDetailType
            {
                Id = 3,
                Title = "Depth",
                Unit = "millimeters"
            },
            new DomainProductDetailType
            {
                Id = 4,
                Title = "Weight",
                Unit = "grams"
            },
            new DomainProductDetailType
            {
                Id = 5,
                Title = "Color",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 6,
                Title = "Material",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 7,
                Title = "Connectivity",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 8,
                Title = "Power",
                Unit = "Watts"
            },
            new DomainProductDetailType
            {
                Id = 9,
                Title = "Battery",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 10,
                Title = "Compatibility",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 11,
                Title = "Connectivity",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 12,
                Title = "Display",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 13,
                Title = "Color Temperature",
                Unit = "Kelvin"
            },
            new DomainProductDetailType
            {
                Id = 14,
                Title = "Video Quality",
                Unit = null
            },
            new DomainProductDetailType
            {
                Id = 15,
                Title = "Audio Quality",
                Unit = null
            }
        );
    }
}