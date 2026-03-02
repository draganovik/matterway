using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

internal sealed class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable(nameof(Payment));

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.OrderId)
            .IsRequired();

        builder.Property(payment => payment.Provider)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(payment => payment.ReferenceId)
            .IsRequired()
            .HasMaxLength(19);

        builder.Property(payment => payment.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .HasMaxLength(20)
            .HasConversion(v => v.ToString(), v => Enum.Parse<EPaymentStatus>(v))
            .IsRequired();

        builder.Property(payment => payment.CreatedAt)
            .IsRequired();
    }
}