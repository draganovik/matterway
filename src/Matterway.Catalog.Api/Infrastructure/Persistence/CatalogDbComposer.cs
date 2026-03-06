using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailNumericEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailTextEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public class CatalogDbComposer(DbContextOptions<CatalogDbComposer> options) : DbContext(options)
{
    public DbSet<Article> Article { get; set; }

    public DbSet<Discount> Discount { get; set; }
    public DbSet<ArticleImage> ArticleImage { get; set; }

    public DbSet<Detail> Detail { get; set; }
    public DbSet<ArticleDetailText> ArticleDetailText { get; set; }
    public DbSet<ArticleDetailNumeric> ArticleDetailNumeric { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ArticleEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new DiscountEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleImageEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new DetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleDetailTextEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleDetailNumericEntityTypeConfiguration());

        ModelDataLoader.Initialize(modelBuilder);
    }
}