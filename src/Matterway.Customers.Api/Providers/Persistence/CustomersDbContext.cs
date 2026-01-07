using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Providers.Persistence.CartItemEntity;
using Matterway.Customers.Api.Providers.Persistence.CustomerEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Providers.Persistence;

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