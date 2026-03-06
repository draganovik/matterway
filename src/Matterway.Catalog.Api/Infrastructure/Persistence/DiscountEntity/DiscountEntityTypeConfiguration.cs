using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;

internal sealed class DiscountEntityTypeConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable(nameof(Discount));

        builder.HasKey(d => new { d.Code, d.ArticleCode });

        builder.Property(d => d.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.ArticleCode)
            .HasMaxLength(ArticleCode.Length)
            .IsRequired();

        builder.Property(d => d.Percentage)
            .IsRequired();

        builder.Property(d => d.ValidFrom)
            .IsRequired();

        builder.Property(d => d.ValidTo);

        builder.HasOne(d => d.Article)
            .WithMany(a => a.Discounts)
            .HasForeignKey(d => d.ArticleCode)
            .HasPrincipalKey(a => a.ArticleCode);
    }
}