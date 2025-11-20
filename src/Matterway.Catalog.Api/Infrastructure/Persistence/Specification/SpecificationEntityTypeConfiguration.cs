using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainSpecification = Matterway.Catalog.Api.Domain.Entities.Specification;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Specification;

internal sealed class SpecificationEntityTypeConfiguration : IEntityTypeConfiguration<DomainSpecification>
{
    public void Configure(EntityTypeBuilder<DomainSpecification> builder)
    {
        builder.ToTable(nameof(DomainSpecification));
        builder.HasKey(s => s.Slug);

        builder.Property(s => s.Slug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(s => s.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(s => s.Unit)
            .HasMaxLength(30);

        builder.HasData(
            new DomainSpecification { Slug = "battery-size", Title = "Battery Size", Unit = "mAh" },
            new DomainSpecification { Slug = "depth", Title = "Depth", Unit = "millimeters" },
            new DomainSpecification { Slug = "height", Title = "Height", Unit = "millimeters" },
            new DomainSpecification { Slug = "power", Title = "Power", Unit = "watts" },
            new DomainSpecification { Slug = "ram-size", Title = "RAM Size", Unit = "GB" },
            new DomainSpecification { Slug = "refresh-rate", Title = "Refresh Rate", Unit = "Hz" },
            new DomainSpecification { Slug = "screen-size", Title = "Screen Size", Unit = "inches" },
            new DomainSpecification { Slug = "storage", Title = "Storage", Unit = "GB" },
            new DomainSpecification { Slug = "weight", Title = "Weight", Unit = "grams" },
            new DomainSpecification { Slug = "width", Title = "Width", Unit = "millimeters" }
        );
    }
}