using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;

internal sealed class CustomerOrderEntityTypeConfiguration : IEntityTypeConfiguration<CustomerOrder>
{
    public void Configure(EntityTypeBuilder<CustomerOrder> builder)
    {
        builder.ToTable(nameof(CustomerOrder));

        builder.HasKey(order => order.OrderId);

        builder.Property(order => order.CustomerId)
            .IsRequired();

        builder.Property(order => order.PlacedAt)
            .IsRequired();

        builder.HasOne(order => order.Customer)
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(order => order.CustomerId);
    }
}