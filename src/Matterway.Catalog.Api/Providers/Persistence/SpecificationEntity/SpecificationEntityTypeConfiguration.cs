using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Providers.Persistence.SpecificationEntity;

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
    }
}