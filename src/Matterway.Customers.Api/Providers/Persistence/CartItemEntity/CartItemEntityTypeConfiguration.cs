using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Customers.Api.Providers.Persistence.CartItemEntity;

internal sealed class CartItemEntityTypeConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(nameof(CartItem),
            tb => { tb.HasCheckConstraint("CK_CartItem_Quantity", "\"Quantity\" >= 1"); });

        // Use composite key: each customer has at most one cart item per product
        builder.HasKey(ci => new { ci.CustomerId, ci.ProductId });

        // Quantity: required and must be >= 1
        builder.Property(ci => ci.Quantity)
            .IsRequired();

        // UnitPrice: optional to align with nullable prices, set precision (best-effort mapping)
        builder.Property(ci => ci.UnitPrice)
            .HasPrecision(18, 2);

        // ProductName: required with a reasonable max length
        builder.Property(ci => ci.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        // Foreign key to Customer
        builder.HasOne(ci => ci.Customer)
            .WithMany() // keep it non-breaking if Customer.CartItems navigation doesn't exist
            .HasForeignKey(ci => ci.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for product lookup within a customer cart
        builder.HasIndex(ci => ci.CustomerId);
    }
}