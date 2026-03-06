using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;

internal sealed class ArticleDetailTextEntityTypeConfiguration : IEntityTypeConfiguration<ArticleDetailText>
{
    public void Configure(EntityTypeBuilder<ArticleDetailText> builder)
    {
        builder.ToTable(nameof(ArticleDetailText));

        builder.HasKey(pd => new { pd.ArticleCode, pd.DetailSlug });

        builder.Property(pd => pd.ArticleCode)
            .HasMaxLength(ArticleCode.Length)
            .IsRequired();

        builder.Property(pd => pd.DetailSlug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(pd => pd.Value)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(pd => pd.Detail)
            .WithMany()
            .HasPrincipalKey(d => d.Slug)
            .HasForeignKey(pd => pd.DetailSlug);

        builder.HasOne(pd => pd.Article)
            .WithMany(p => p.ArticleDetailTexts)
            .HasForeignKey(pd => pd.ArticleCode)
            .HasPrincipalKey(p => p.ArticleCode);
    }
}