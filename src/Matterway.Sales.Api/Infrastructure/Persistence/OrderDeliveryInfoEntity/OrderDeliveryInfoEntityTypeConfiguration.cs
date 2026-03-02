using Matterway.Sales.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Sales.Api.Infrastructure.Persistence.OrderDeliveryInfoEntity;

internal sealed class OrderDeliveryInfoEntityTypeConfiguration : IEntityTypeConfiguration<OrderDeliveryInfo>
{
    public void Configure(EntityTypeBuilder<OrderDeliveryInfo> builder)
    {
        builder.ToTable(nameof(OrderDeliveryInfo));

        builder.HasKey(info => info.Id);

        builder.Property(info => info.OrderId)
            .IsRequired();

        builder.HasIndex(info => info.OrderId)
            .IsUnique();

        builder.Property(info => info.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(info => info.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(info => info.ZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(info => info.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(info => info.AddressLine2)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(info => info.ContactPhone)
            .IsRequired(false)
            .HasMaxLength(30);
    }
}