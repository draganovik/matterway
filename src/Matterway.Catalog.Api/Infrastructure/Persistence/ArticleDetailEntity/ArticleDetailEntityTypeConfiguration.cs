using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;

internal sealed class ArticleDetailEntityTypeConfiguration : IEntityTypeConfiguration<ArticleDetail>
{
    public void Configure(EntityTypeBuilder<ArticleDetail> builder)
    {
        builder.ToTable(nameof(ArticleDetail));

        builder.HasKey(pd => new { pd.ArticleId, pd.DetailSlug });

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
            .WithMany(p => p.ArticleDetails)
            .HasForeignKey(pd => pd.ArticleId);
    }
}