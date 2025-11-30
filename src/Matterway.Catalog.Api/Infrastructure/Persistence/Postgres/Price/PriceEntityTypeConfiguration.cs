using Matterway.Catalog.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainPrice = Matterway.Catalog.Api.Domain.Entities.Price;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Price;

internal sealed class PriceEntityTypeConfiguration : IEntityTypeConfiguration<DomainPrice>
{
    public void Configure(EntityTypeBuilder<DomainPrice> builder)
    {
        builder.ToTable(nameof(DomainPrice));

        builder.HasKey(p => new { p.ProductId, p.Currency });
        builder.Property(p => p.ProductId);

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .HasConversion(v => v.ToString(), v => Enum.Parse<ESupportedCurrency>(v))
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired();

        builder.HasOne(p => p.Product)
            .WithMany(p => p.Prices)
            .HasForeignKey(p => p.ProductId);

        builder.HasData(
            new DomainPrice
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Currency = ESupportedCurrency.RSD,
                Amount = 4999
            },
            new DomainPrice
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Currency = ESupportedCurrency.RSD,
                Amount = 24999
            },
            new DomainPrice
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Currency = ESupportedCurrency.RSD,
                Amount = 27999
            },
            new DomainPrice
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Currency = ESupportedCurrency.RSD,
                Amount = 9999
            },
            new DomainPrice
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Currency = ESupportedCurrency.RSD,
                Amount = 19999
            }
        );
    }
}