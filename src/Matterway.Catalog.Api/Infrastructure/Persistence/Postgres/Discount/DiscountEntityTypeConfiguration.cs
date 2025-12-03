using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainDiscount = Matterway.Catalog.Api.Domain.Entities.Discount;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Discount;

internal sealed class DiscountEntityTypeConfiguration : IEntityTypeConfiguration<DomainDiscount>
{
    public void Configure(EntityTypeBuilder<DomainDiscount> builder)
    {
        builder.ToTable(nameof(DomainDiscount));

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
            new DomainDiscount
            {
                Code = "WINTER25",
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Percentage = 0.25m,
                ValidFrom = new DateTime(2025, 11, 20).ToUniversalTime(),
                ValidTo = new DateTime(2026, 3, 20).ToUniversalTime()
            }
        );
    }
}