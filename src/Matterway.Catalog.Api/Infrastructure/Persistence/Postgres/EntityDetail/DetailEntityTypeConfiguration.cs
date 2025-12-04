using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityDetail;

internal sealed class DetailEntityTypeConfiguration : IEntityTypeConfiguration<Detail>
{
    public void Configure(EntityTypeBuilder<Detail> builder)
    {
        builder.ToTable(nameof(Detail));
        builder.HasKey(d => d.Slug);

        builder.Property(d => d.Slug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(d => d.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasOne<AttributeSlug>()
            .WithMany()
            .HasPrincipalKey(a => a.Slug)
            .HasForeignKey(d => d.Slug)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Detail { Slug = "audio", Title = "Audio" },
            new Detail { Slug = "audio-quality", Title = "Audio Quality" },
            new Detail { Slug = "battery", Title = "Battery" },
            new Detail { Slug = "brand", Title = "Brand" },
            new Detail { Slug = "camera", Title = "Camera" },
            new Detail { Slug = "color", Title = "Color" },
            new Detail { Slug = "color-temperature", Title = "Color Temperature" },
            new Detail { Slug = "compatibility", Title = "Compatibility" },
            new Detail { Slug = "connectivity", Title = "Connectivity" },
            new Detail { Slug = "display", Title = "Display" },
            new Detail { Slug = "features", Title = "Features" },
            new Detail { Slug = "material", Title = "Material" },
            new Detail { Slug = "model", Title = "Model" },
            new Detail { Slug = "operating-system", Title = "Operating System" },
            new Detail { Slug = "ports", Title = "Ports" },
            new Detail { Slug = "processor", Title = "Processor" },
            new Detail { Slug = "resolution", Title = "Resolution" },
            new Detail { Slug = "video-quality", Title = "Video Quality" }
        );
    }
}