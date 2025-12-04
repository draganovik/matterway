using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityAttributeSlug;

internal sealed class AttributeSlugEntityTypeConfiguration : IEntityTypeConfiguration<AttributeSlug>
{
    public void Configure(EntityTypeBuilder<AttributeSlug> builder)
    {
        builder.ToTable(nameof(AttributeSlug));
        builder.HasKey(a => a.Slug);

        builder.Property(a => a.Slug)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(a => a.Slug)
            .IsUnique();

        builder.HasData(
            // Details
            new AttributeSlug { Slug = "audio" },
            new AttributeSlug { Slug = "audio-quality" },
            new AttributeSlug { Slug = "battery" },
            new AttributeSlug { Slug = "brand" },
            new AttributeSlug { Slug = "camera" },
            new AttributeSlug { Slug = "color" },
            new AttributeSlug { Slug = "color-temperature" },
            new AttributeSlug { Slug = "compatibility" },
            new AttributeSlug { Slug = "connectivity" },
            new AttributeSlug { Slug = "display" },
            new AttributeSlug { Slug = "features" },
            new AttributeSlug { Slug = "material" },
            new AttributeSlug { Slug = "model" },
            new AttributeSlug { Slug = "operating-system" },
            new AttributeSlug { Slug = "ports" },
            new AttributeSlug { Slug = "processor" },
            new AttributeSlug { Slug = "resolution" },
            new AttributeSlug { Slug = "video-quality" },
            // Specifications
            new AttributeSlug { Slug = "battery-size" },
            new AttributeSlug { Slug = "depth" },
            new AttributeSlug { Slug = "height" },
            new AttributeSlug { Slug = "power" },
            new AttributeSlug { Slug = "ram-size" },
            new AttributeSlug { Slug = "refresh-rate" },
            new AttributeSlug { Slug = "screen-size" },
            new AttributeSlug { Slug = "storage" },
            new AttributeSlug { Slug = "weight" },
            new AttributeSlug { Slug = "width" }
        );
    }
}