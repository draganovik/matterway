using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.EntityAttributeSlug;

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
    }
}