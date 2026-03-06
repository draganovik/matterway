using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;

internal sealed class ArticleImageEntityTypeConfiguration : IEntityTypeConfiguration<ArticleImage>
{
    public void Configure(EntityTypeBuilder<ArticleImage> builder)
    {
        builder.ToTable(nameof(ArticleImage));

        builder.HasKey(pi => new { pi.Id, pi.ArticleCode });

        builder.Property(pi => pi.OrderIndex)
            .IsRequired();

        builder.Property(pi => pi.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pi => pi.ImageAlt)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pi => pi.ArticleCode)
            .HasMaxLength(ArticleCode.Length)
            .IsRequired();

        builder.HasIndex(pi => new { pi.ArticleCode, pi.OrderIndex }).IsUnique();

        builder.HasOne(pi => pi.Article)
            .WithMany(p => p.ArticleImages!)
            .HasForeignKey(pi => pi.ArticleCode)
            .HasPrincipalKey(p => p.ArticleCode);
    }
}