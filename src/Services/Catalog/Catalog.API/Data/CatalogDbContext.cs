using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Entities;

namespace Catalog.API.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext (DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public DbSet<Catalog.API.Entities.ProductDetail> ProductDetail { get; set; } = default!;
    }
}
