using Matterway.Catalog.Api.Infrastructure.Persistence.Product;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetailType;
using Microsoft.EntityFrameworkCore;
using DomainProduct = Matterway.Catalog.Api.Domain.Entities.Product;
using DomainProductDetail = Matterway.Catalog.Api.Domain.Entities.ProductDetail;
using DomainProductImage = Matterway.Catalog.Api.Domain.Entities.ProductImage;
using DomainProductDetailType = Matterway.Catalog.Api.Domain.Entities.ProductDetailType;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public class CatalogDb(DbContextOptions<CatalogDb> options) : DbContext(options)
{
    public DbSet<DomainProductDetail> ProductDetail { get; set; }

    public DbSet<DomainProductImage> ProductImage { get; set; }

    public DbSet<DomainProduct> Product { get; set; }

    public DbSet<DomainProductDetailType> ProductDetailType { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductDetailEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductDetailTypeEntityTypeConfiguration());
    }
}