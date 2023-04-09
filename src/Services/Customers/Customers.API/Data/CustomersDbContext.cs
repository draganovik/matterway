using Customers.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customers.API.Data;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customer { get; set; } = default!;
}
