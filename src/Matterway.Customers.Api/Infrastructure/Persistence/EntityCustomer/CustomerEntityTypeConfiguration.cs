using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Customers.Api.Infrastructure.Persistence.EntityCustomer;

internal sealed class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable(nameof(Customer));

        // Primary key
        builder.HasKey(c => c.Id);

        // Unique SystemUserId
        builder.HasIndex(c => c.SystemUserId)
            .IsUnique();

        builder.Property(c => c.SystemUserId)
            .IsRequired();

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.BirthDate)
            .IsRequired();

        builder.Property(c => c.DefaultAddressId)
            .IsRequired(false);
    }
}