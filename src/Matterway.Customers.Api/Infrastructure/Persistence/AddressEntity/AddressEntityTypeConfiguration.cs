using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;

internal sealed class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable(nameof(Address));

        builder.HasKey(address => address.Id);

        builder.Property(address => address.CustomerId)
            .IsRequired();

        builder.HasIndex(address => address.CustomerId)
            .IsUnique();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(address => address.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(address => address.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(address => address.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(address => address.ZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(address => address.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(address => address.AddressLine2)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(address => address.ContactPhone)
            .IsRequired()
            .HasMaxLength(30);
    }
}