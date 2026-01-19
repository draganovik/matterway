using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Sales.Api.Providers.Persistence.OrderEntity;

internal sealed class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(nameof(Order));

        builder.HasKey(order => order.Id);

        builder.Property(order => order.CustomerId)
            .IsRequired(false);

        builder.Property(order => order.PlacedAt)
            .IsRequired();

        builder.Property(order => order.Type)
            .HasMaxLength(20)
            .HasConversion(v => v.ToString(), v => Enum.Parse<EOrderType>(v))
            .IsRequired();

        builder.HasOne(order => order.DeliveryInfo)
            .WithOne(info => info.Order)
            .HasForeignKey<OrderDeliveryInfo>(info => info.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(order => order.StatusHistory)
            .WithOne(status => status.Order)
            .HasForeignKey(status => status.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(order => order.Payments)
            .WithOne(payment => payment.Order)
            .HasForeignKey(payment => payment.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}