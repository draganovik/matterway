using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainAttributeSlug = Matterway.Catalog.Api.Domain.Entities.AttributeSlug;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.AttributeSlug;

internal sealed class AttributeSlugEntityTypeConfiguration : IEntityTypeConfiguration<DomainAttributeSlug>
{
    public void Configure(EntityTypeBuilder<DomainAttributeSlug> builder)
    {
        builder.ToTable(nameof(DomainAttributeSlug));
        builder.HasKey(a => a.Slug);

        builder.Property(a => a.Slug)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(a => a.Slug)
            .IsUnique();

        builder.HasData(
            // Details
            new DomainAttributeSlug { Slug = "audio" },
            new DomainAttributeSlug { Slug = "audio-quality" },
            new DomainAttributeSlug { Slug = "battery" },
            new DomainAttributeSlug { Slug = "brand" },
            new DomainAttributeSlug { Slug = "camera" },
            new DomainAttributeSlug { Slug = "color" },
            new DomainAttributeSlug { Slug = "color-temperature" },
            new DomainAttributeSlug { Slug = "compatibility" },
            new DomainAttributeSlug { Slug = "connectivity" },
            new DomainAttributeSlug { Slug = "display" },
            new DomainAttributeSlug { Slug = "features" },
            new DomainAttributeSlug { Slug = "material" },
            new DomainAttributeSlug { Slug = "model" },
            new DomainAttributeSlug { Slug = "operating-system" },
            new DomainAttributeSlug { Slug = "ports" },
            new DomainAttributeSlug { Slug = "processor" },
            new DomainAttributeSlug { Slug = "resolution" },
            new DomainAttributeSlug { Slug = "video-quality" },
            // Specifications
            new DomainAttributeSlug { Slug = "battery-size" },
            new DomainAttributeSlug { Slug = "depth" },
            new DomainAttributeSlug { Slug = "height" },
            new DomainAttributeSlug { Slug = "power" },
            new DomainAttributeSlug { Slug = "ram-size" },
            new DomainAttributeSlug { Slug = "refresh-rate" },
            new DomainAttributeSlug { Slug = "screen-size" },
            new DomainAttributeSlug { Slug = "storage" },
            new DomainAttributeSlug { Slug = "weight" },
            new DomainAttributeSlug { Slug = "width" }
        );
    }
}