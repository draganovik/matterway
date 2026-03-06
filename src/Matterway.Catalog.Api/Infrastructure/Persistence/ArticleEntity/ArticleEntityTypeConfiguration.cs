using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

internal sealed class ArticleEntityTypeConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable(nameof(Article));

        builder.HasKey(p => p.ArticleCode);

        builder.Property(p => p.ArticleCode)
            .HasMaxLength(ArticleCode.Length)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.BasePrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.IsAvailable)
            .IsRequired();

        builder.HasMany(p => p.ArticleDetailTexts)
            .WithOne(pd => pd.Article)
            .HasForeignKey(pd => pd.ArticleCode)
            .HasPrincipalKey(p => p.ArticleCode);

        builder.HasMany(p => p.ArticleDetailNumerics)
            .WithOne(pd => pd.Article)
            .HasForeignKey(pd => pd.ArticleCode)
            .HasPrincipalKey(p => p.ArticleCode);

        builder.HasMany(p => p.ArticleImages)
            .WithOne(pi => pi.Article)
            .HasForeignKey(pi => pi.ArticleCode)
            .HasPrincipalKey(p => p.ArticleCode);

        builder.HasMany(p => p.Discounts)
            .WithOne(d => d.Article)
            .HasForeignKey(d => d.ArticleCode)
            .HasPrincipalKey(p => p.ArticleCode);
    }
}