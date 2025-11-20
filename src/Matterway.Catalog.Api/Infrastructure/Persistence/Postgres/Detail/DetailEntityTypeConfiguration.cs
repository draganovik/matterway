using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainDetail = Matterway.Catalog.Api.Domain.Entities.Detail;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Detail;

internal sealed class DetailEntityTypeConfiguration : IEntityTypeConfiguration<DomainDetail>
{
    public void Configure(EntityTypeBuilder<DomainDetail> builder)
    {
        builder.ToTable(nameof(DomainDetail));
        builder.HasKey(d => d.Slug);

        builder.Property(d => d.Slug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(d => d.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasData(
            new DomainDetail { Slug = "audio", Title = "Audio" },
            new DomainDetail { Slug = "audio-quality", Title = "Audio Quality" },
            new DomainDetail { Slug = "battery", Title = "Battery" },
            new DomainDetail { Slug = "brand", Title = "Brand" },
            new DomainDetail { Slug = "camera", Title = "Camera" },
            new DomainDetail { Slug = "color", Title = "Color" },
            new DomainDetail { Slug = "color-temperature", Title = "Color Temperature" },
            new DomainDetail { Slug = "compatibility", Title = "Compatibility" },
            new DomainDetail { Slug = "connectivity", Title = "Connectivity" },
            new DomainDetail { Slug = "display", Title = "Display" },
            new DomainDetail { Slug = "features", Title = "Features" },
            new DomainDetail { Slug = "material", Title = "Material" },
            new DomainDetail { Slug = "model", Title = "Model" },
            new DomainDetail { Slug = "operating-system", Title = "Operating System" },
            new DomainDetail { Slug = "ports", Title = "Ports" },
            new DomainDetail { Slug = "processor", Title = "Processor" },
            new DomainDetail { Slug = "resolution", Title = "Resolution" },
            new DomainDetail { Slug = "video-quality", Title = "Video Quality" }
        );
    }
}