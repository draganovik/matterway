using Catalog.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductDetail> ProductDetail { get; set; } = default!;

    public DbSet<ProductImage> ProductImage { get; set; } = default!;

    public DbSet<Product> Product { get; set; } = default!;
}
