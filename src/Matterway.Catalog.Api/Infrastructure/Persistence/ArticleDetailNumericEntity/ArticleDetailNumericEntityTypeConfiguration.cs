using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;

internal sealed class ArticleDetailNumericEntityTypeConfiguration : IEntityTypeConfiguration<ArticleDetailNumeric>
{
    public void Configure(EntityTypeBuilder<ArticleDetailNumeric> builder)
    {
        builder.ToTable(nameof(ArticleDetailNumeric));

        builder.HasKey(pd => new { pd.ArticleId, pd.DetailSlug });

        builder.Property(pd => pd.DetailSlug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(pd => pd.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(pd => pd.Detail)
            .WithMany()
            .HasPrincipalKey(d => d.Slug)
            .HasForeignKey(pd => pd.DetailSlug);

        builder.HasOne(pd => pd.Article)
            .WithMany(p => p.ArticleDetailNumerics)
            .HasForeignKey(pd => pd.ArticleId);
    }
}