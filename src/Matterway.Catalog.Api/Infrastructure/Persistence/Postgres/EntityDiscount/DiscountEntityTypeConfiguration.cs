using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityDiscount;

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

        builder.HasData(
            new Discount
            {
                Code = "WINTER25",
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Currency = ESupportedCurrency.RSD,
                Percentage = 0.25m,
                ValidFrom = new DateTime(2025, 11, 20).ToUniversalTime(),
                ValidTo = new DateTime(2026, 3, 20).ToUniversalTime()
            });
    }
}