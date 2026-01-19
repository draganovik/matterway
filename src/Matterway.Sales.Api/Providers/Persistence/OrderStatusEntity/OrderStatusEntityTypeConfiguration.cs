using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Sales.Api.Providers.Persistence.OrderStatusEntity;

internal sealed class OrderStatusEntityTypeConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable(nameof(OrderStatus));

        builder.HasKey(status => status.Id);

        builder.Property(status => status.OrderId)
            .IsRequired();

        builder.Property(status => status.ChangedAt)
            .IsRequired();

        builder.Property(status => status.Status)
            .HasMaxLength(20)
            .HasConversion(v => v.ToString(), v => Enum.Parse<EOrderStatusType>(v))
            .IsRequired();

        builder.Property(status => status.Note)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}