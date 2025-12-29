using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.AttributeSlugEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.DiscountEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.PriceEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImageEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductSpecificationEntity;
using Matterway.Catalog.Api.Infrastructure.Persistence.SpecificationEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public class CatalogDb(DbContextOptions<CatalogDb> options) : DbContext(options)
{
    public DbSet<Product> Product { get; set; }

    public DbSet<Price> Price { get; set; }
    public DbSet<Discount> Discount { get; set; }
    public DbSet<ProductImage> ProductImage { get; set; }

    public DbSet<AttributeSlug> AttributeSlug { get; set; }

    public DbSet<Detail> Detail { get; set; }
    public DbSet<ProductDetail> ProductDetail { get; set; }

    public DbSet<Specification> Specification { get; set; }
    public DbSet<ProductSpecification> ProductSpecification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new PriceEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new AttributeSlugEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new DetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductDetailEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new SpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductSpecificationEntityTypeConfiguration());

        ModelDataLoader.InitializeDemo(modelBuilder);
    }
}