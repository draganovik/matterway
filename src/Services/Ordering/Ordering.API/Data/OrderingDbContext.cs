using Microsoft.EntityFrameworkCore;
using Ordering.API.Entities;

namespace Ordering.API.Data;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Address { get; set; } = default!;

    public DbSet<OrderHistory> OrderHistory { get; set; } = default!;

    public DbSet<Order> Order { get; set; } = default!;

    public DbSet<Ordering.API.Entities.OrderItem> OrderItem { get; set; } = default!;
}
