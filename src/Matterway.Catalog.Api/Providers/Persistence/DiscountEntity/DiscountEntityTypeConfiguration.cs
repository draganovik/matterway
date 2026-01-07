using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Providers.Persistence.DiscountEntity;

internal sealed class DiscountEntityTypeConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable(nameof(Discount));

        builder.HasKey(d => new { d.Code, d.ProductId, d.Currency });

        builder.Property(d => d.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.Percentage)
            .IsRequired();

        builder.Property(d => d.ValidFrom)
            .IsRequired();

        builder.Property(d => d.ValidTo);

        builder.HasOne(d => d.Price)
            .WithMany(p => p.Discounts)
            .HasForeignKey(d => new { d.ProductId, d.Currency });
    }
}