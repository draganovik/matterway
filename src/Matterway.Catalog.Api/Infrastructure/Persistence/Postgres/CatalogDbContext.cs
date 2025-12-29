using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityAttributeSlug;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityDiscount;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityPrice;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProduct;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductImage;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductSpecification;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntitySpecification;
using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres;

public class CatalogDb(DbContextOptions<CatalogDb> options) : DbContext(options)
{
    public DbSet<AttributeSlug> AttributeSlug { get; set; }

    public DbSet<Detail> Detail { get; set; }

    public DbSet<Discount> Discount { get; set; }

    public DbSet<ProductDetail> ProductDetail { get; set; }

    public DbSet<ProductImage> ProductImage { get; set; }

    public DbSet<Product> Product { get; set; }

    public DbSet<ProductSpecification> ProductSpecification { get; set; }

    public DbSet<Price> Price { get; set; }

    public DbSet<Specification> Specification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductDetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductSpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PriceEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AttributeSlugEntityTypeConfiguration());
    }
}