using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;

internal sealed class ArticleEntityTypeConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable(nameof(Article));

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.ArticleCode)
            .IsUnique();

        builder.Property(p => p.ArticleCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.IsAvailable)
            .IsRequired();

        builder.HasMany(p => p.ArticleDetails)
            .WithOne(pd => pd.Article)
            .HasForeignKey(pd => pd.ArticleId);

        builder.HasMany(p => p.ArticleSpecifications)
            .WithOne(ps => ps.Article)
            .HasForeignKey(ps => ps.ArticleId);

        builder.HasMany(p => p.ArticleImages)
            .WithOne(pi => pi.Article)
            .HasForeignKey(pi => pi.ArticleId);
    }
}