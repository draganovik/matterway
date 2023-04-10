using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Data
{
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ordering.API.Entities.Address> Address { get; set; } = default!;
    }
}
