using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence;

public class CustomersDbComposer(DbContextOptions<CustomersDbComposer> options) : DbContext(options)
{
    public DbSet<Customer> Customer { get; set; }

    public DbSet<CustomerArticle> CustomerArticle { get; set; }

    public DbSet<CustomerOrder> CustomerOrder { get; set; }

    public DbSet<Address> Address { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AddressEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerArticleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerOrderEntityTypeConfiguration());

        ModelDataLoader.InitializeDemo(modelBuilder);
    }
}