using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.PriceEntity;

internal sealed class PriceEntityTypeConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.ToTable(nameof(Price));

        builder.HasKey(p => new { p.ArticleId, p.Currency });
        builder.Property(p => p.ArticleId);

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .HasConversion(v => v.ToString(), v => Enum.Parse<ESupportedCurrency>(v))
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired();

        builder.HasOne(p => p.Article)
            .WithMany(p => p.Prices)
            .HasForeignKey(p => p.ArticleId);
    }
}