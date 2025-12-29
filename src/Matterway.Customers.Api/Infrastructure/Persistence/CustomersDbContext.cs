using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.EntityCartItem;
using Matterway.Customers.Api.Infrastructure.Persistence.EntityCustomer;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence;

public class CustomersDb(DbContextOptions<CustomersDb> options) : DbContext(options)
{
    public DbSet<Customer> Customer { get; set; }

    public DbSet<CartItem> CartItem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemEntityTypeConfiguration());

        ModelDataLoader.InitializeDemo(modelBuilder);
    }
}