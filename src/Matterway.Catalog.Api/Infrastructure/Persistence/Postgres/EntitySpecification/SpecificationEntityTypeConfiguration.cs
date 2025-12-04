using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntitySpecification;

internal sealed class SpecificationEntityTypeConfiguration : IEntityTypeConfiguration<Specification>
{
    public void Configure(EntityTypeBuilder<Specification> builder)
    {
        builder.ToTable(nameof(Specification));
        builder.HasKey(s => s.Slug);

        builder.Property(s => s.Slug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(s => s.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(s => s.Unit)
            .HasMaxLength(30);

        builder.HasOne<AttributeSlug>()
            .WithMany()
            .HasPrincipalKey(a => a.Slug)
            .HasForeignKey(s => s.Slug)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Specification { Slug = "battery-size", Title = "Battery Size", Unit = "mAh" },
            new Specification { Slug = "depth", Title = "Depth", Unit = "millimeters" },
            new Specification { Slug = "height", Title = "Height", Unit = "millimeters" },
            new Specification { Slug = "power", Title = "Power", Unit = "watts" },
            new Specification { Slug = "ram-size", Title = "RAM Size", Unit = "GB" },
            new Specification { Slug = "refresh-rate", Title = "Refresh Rate", Unit = "Hz" },
            new Specification { Slug = "screen-size", Title = "Screen Size", Unit = "inches" },
            new Specification { Slug = "storage", Title = "Storage", Unit = "GB" },
            new Specification { Slug = "weight", Title = "Weight", Unit = "grams" },
            new Specification { Slug = "width", Title = "Width", Unit = "millimeters" }
        );
    }
}