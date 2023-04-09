using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Catalog.API.Entities.ProductDetail> ProductDetail { get; set; } = default!;
    }
}
