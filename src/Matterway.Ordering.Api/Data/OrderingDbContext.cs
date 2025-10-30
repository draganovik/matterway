using Microsoft.EntityFrameworkCore;
using Matterway.Common.Enums;
using Matterway.Ordering.Api.Features.Addresses.Domain;
using Matterway.Ordering.Api.Features.OrderHistories.Domain;
using Matterway.Ordering.Api.Features.OrderItems.Domain;
using Matterway.Ordering.Api.Features.Orders.Domain;

namespace Matterway.Ordering.Api.Data;

public class OrderingDb : DbContext
{
    public OrderingDb(DbContextOptions<OrderingDb> options)
        : base(options)
    {
    }

    public DbSet<Address> Address { get; set; } = default!;

    public DbSet<OrderHistory> OrderHistory { get; set; } = default!;

    public DbSet<Order> Order { get; set; } = default!;

    public DbSet<OrderItem> OrderItem { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>().HasData(
            new Address
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                Street = "Njegoševa",
                Residence = "54",
                City = "Sremska Mitrovica",
                ZipCode = "22000",
                ReceiverName = "Mara Jakov"
            },
            new Address
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                Street = "Narodnih Heroja",
                Residence = "3",
                City = "Novi Sad",
                ZipCode = "21000",
                ReceiverName = "Stefan Stefanov"
            }
        );
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                DeliveryAddressId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"),
                ReferenceNumber = "5655-6666-7877"
            },
            new Order
            {
                DeliveryAddressId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"),
                ReferenceNumber = "6666-8888-6588"
            }
        );
        modelBuilder.Entity<OrderItem>().HasData(
            new OrderItem
            {
                OrderId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                ProductName = "Ring Spotlight Cam",
                UnitPrice = 19999,
                Quantity = 2
            },
            new OrderItem
            {
                OrderId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                ProductName = "Philips Hue White and Color Ambiance A19 Smart LED Bulb",
                UnitPrice = 4999,
                Quantity = 1
            }
        );
        modelBuilder.Entity<OrderHistory>().HasData(
            new OrderHistory
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"),
                OrderId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"),
                OrderStatus = OrderStatus.Ready,
                Description = "Order Ready",
                CreatedDate = DateTime.Parse("2024-06-01T12:00:00")
            },
            new OrderHistory
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b8"),
                OrderId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"),
                OrderStatus = OrderStatus.Canceled,
                Description = "Order Canceled",
                CreatedDate = DateTime.Parse("2024-06-02T14:30:00")
            }
        );
    }
}