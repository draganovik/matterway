using Matterway.Sales.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Sales.Api.Infrastructure.Persistence.OrderItemEntity;

internal sealed class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(nameof(OrderItem));

        builder.HasKey(item => item.Id);

        builder.Property(item => item.OrderId)
            .HasMaxLength(OrderId.MaxLength)
            .HasConversion(orderId => orderId.Value, value => OrderId.FromStorage(value))
            .IsRequired();

        builder.Property(item => item.ArticleCode)
            .IsRequired()
            .HasMaxLength(ArticleCode.Length);

        builder.Property(item => item.ArticleTitle)
            .IsRequired();
        builder.Property(item => item.ArticleTitle)
            .HasMaxLength(200);

        builder.Property(item => item.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .IsRequired();
    }
}