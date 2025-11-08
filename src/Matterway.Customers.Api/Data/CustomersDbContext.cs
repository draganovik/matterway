using Matterway.Customers.Api.Features.CartItems.Domain;
using Matterway.Customers.Api.Features.Customers.Domain;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Data;

public class CustomersDb : DbContext
{
    public CustomersDb(DbContextOptions<CustomersDb> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customer { get; set; } = default!;

    public DbSet<CartItem> CartItem { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                FirstName = "Stefan",
                LastName = "Stefanov",
                BirthDate = new DateOnly(1980, 1, 1),
                SystemUserId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3")
            },
            new Customer
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                FirstName = "Mara",
                LastName = "Jakov",
                BirthDate = new DateOnly(2000, 5, 5),
                SystemUserId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4")
            }
        );
        modelBuilder.Entity<CartItem>().HasData(
            new CartItem
            {
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                ProductName = "Philips Hue White and Color Ambiance A19 Smart LED Bulb",
                UnitPrice = 4999,
                Quantity = 3
            },
            new CartItem
            {
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                ProductName = "Ring Spotlight Cam",
                UnitPrice = 19999,
                Quantity = 1
            }
        );
    }
}