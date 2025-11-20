using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Detail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Product;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.ProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.ProductImage;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.ProductSpecification;
using Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Specification;
using Microsoft.EntityFrameworkCore;
using DomainProduct = Matterway.Catalog.Api.Domain.Entities.Product;
using DomainProductDetail = Matterway.Catalog.Api.Domain.Entities.ProductDetail;
using DomainProductImage = Matterway.Catalog.Api.Domain.Entities.ProductImage;
using DomainDetail = Matterway.Catalog.Api.Domain.Entities.Detail;
using DomainProductSpecification = Matterway.Catalog.Api.Domain.Entities.ProductSpecification;
using DomainSpecification = Matterway.Catalog.Api.Domain.Entities.Specification;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres;

public class CatalogDb(DbContextOptions<CatalogDb> options) : DbContext(options)
{
    public DbSet<DomainDetail> Detail { get; set; }

    public DbSet<DomainProductDetail> ProductDetail { get; set; }

    public DbSet<DomainProductImage> ProductImage { get; set; }

    public DbSet<DomainProduct> Product { get; set; }

    public DbSet<DomainProductSpecification> ProductSpecification { get; set; }

    public DbSet<DomainSpecification> Specification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductDetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductSpecificationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SpecificationEntityTypeConfiguration());
    }
}