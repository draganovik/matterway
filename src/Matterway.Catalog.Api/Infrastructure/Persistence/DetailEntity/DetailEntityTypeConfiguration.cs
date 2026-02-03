using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;

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

        builder.Property(d => d.Unit)
            .HasMaxLength(40);
    }
}