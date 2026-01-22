using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleDetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleImageEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ArticleSpecificationEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.AttributeSlugEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.SpecificationEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public class CatalogDbComposer(DbContextOptions<CatalogDbComposer> options) : DbContext(options)
{
    public DbSet<Article> Article { get; set; }

    public DbSet<Discount> Discount { get; set; }
    public DbSet<ArticleImage> ArticleImage { get; set; }

    public DbSet<AttributeSlug> AttributeSlug { get; set; }

    public DbSet<Detail> Detail { get; set; }
    public DbSet<ArticleDetail> ArticleDetail { get; set; }

    public DbSet<Specification> Specification { get; set; }
    public DbSet<ArticleSpecification> ArticleSpecification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ArticleEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new DiscountEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleImageEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new AttributeSlugEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new DetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleDetailEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new SpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleSpecificationEntityTypeConfiguration());

        ModelDataLoader.InitializeDemo(modelBuilder);
    }
}