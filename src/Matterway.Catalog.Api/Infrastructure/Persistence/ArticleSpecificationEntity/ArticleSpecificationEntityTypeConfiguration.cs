using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;

internal sealed class ArticleSpecificationEntityTypeConfiguration : IEntityTypeConfiguration<ArticleSpecification>
{
    public void Configure(EntityTypeBuilder<ArticleSpecification> builder)
    {
        builder.ToTable(nameof(ArticleSpecification));

        builder.HasKey(ps => new { ps.ArticleId, ps.SpecificationSlug });

        builder.Property(ps => ps.SpecificationSlug)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(ps => ps.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(ps => ps.Specification)
            .WithMany()
            .HasPrincipalKey(s => s.Slug)
            .HasForeignKey(ps => ps.SpecificationSlug);

        builder.HasOne(ps => ps.Article)
            .WithMany(p => p.ArticleSpecifications)
            .HasForeignKey(ps => ps.ArticleId);
    }
}