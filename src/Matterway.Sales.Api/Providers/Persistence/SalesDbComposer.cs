using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Providers.Persistence.OrderDeliveryInfoEntity;
using Matterway.Sales.Api.Providers.Persistence.OrderEntity;
using Matterway.Sales.Api.Providers.Persistence.OrderItemEntity;
using Matterway.Sales.Api.Providers.Persistence.OrderStatusEntity;
using Matterway.Sales.Api.Providers.Persistence.PaymentEntity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Sales.Api.Providers.Persistence;

public class SalesDbComposer(DbContextOptions<SalesDbComposer> options) : DbContext(options)
{
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderDeliveryInfo> OrderDeliveryInfo { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    public DbSet<OrderStatus> OrderStatus { get; set; }
    public DbSet<Payment> Payment { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderDeliveryInfoEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
    }
}