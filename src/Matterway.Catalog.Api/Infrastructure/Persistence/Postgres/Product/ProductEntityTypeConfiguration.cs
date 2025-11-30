using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.Product;

using DomainProduct = Domain.Entities.Product;

internal sealed class ProductEntityTypeConfiguration : IEntityTypeConfiguration<DomainProduct>
{
    public void Configure(EntityTypeBuilder<DomainProduct> builder)
    {
        builder.ToTable(nameof(DomainProduct));

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.ProductCode)
            .IsUnique();


        builder.Property(p => p.ProductCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        builder.Property(p => p.IsAvailable)
            .IsRequired();

        builder.HasMany(p => p.ProductDetails)
            .WithOne(pd => pd.Product)
            .HasForeignKey(pd => pd.ProductId);

        builder.HasMany(p => p.ProductSpecifications)
            .WithOne(ps => ps.Product)
            .HasForeignKey(ps => ps.ProductId);

        builder.HasMany(p => p.ProductImages)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId);

        builder.HasData(
            new DomainProduct
            {
                Id = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Title = "Philips Hue White and Color Ambiance A19 Smart LED Bulb",
                CreatedAt = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                Description =
                    "The Philips Hue White and Color Ambiance A19 Smart LED Bulb lets you control your lights from your smartphone or tablet. Choose from 16 million colors to match the mood of any room, and set the lights to turn on and off on a schedule or when you're away from home.",
                ProductCode = "PH002",
                IsAvailable = true
            },
            new DomainProduct
            {
                Id = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                CreatedAt = new DateTime(2024, 6, 2, 14, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 2, 14, 30, 0, DateTimeKind.Utc),
                Title = "Nest Learning Thermostat",
                Description =
                    "The 3rd generation Nest Learning Thermostat programs itself and automatically saves energy when you're away. It learns what temperature you like and builds a schedule around yours.",
                ProductCode = "NT003",
                IsAvailable = true
            },
            new DomainProduct
            {
                Id = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Title = "August Wi-Fi Smart Lock Pro",
                CreatedAt = new DateTime(2024, 6, 3, 16, 45, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 3, 16, 45, 0, DateTimeKind.Utc),
                Description =
                    "The August Wi-Fi Smart Lock Pro + Connect lets you add secure keyless entry to your home. Lock and unlock your door with your phone, and give keyless entry to family, friends, housekeepers, and other home services without worrying about lost or copied keys.",
                ProductCode = "AL001",
                IsAvailable = true
            },
            new DomainProduct
            {
                Id = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Title = "Amazon Echo (4th Gen)",
                CreatedAt = new DateTime(2024, 6, 4, 11, 15, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 4, 11, 15, 0, DateTimeKind.Utc),
                Description =
                    "The Amazon Echo (4th Gen) is a hands-free smart speaker that you control with your voice. It connects to Alexa to play music, make calls, set alarms and timers, ask questions, control smart home devices, and more.",
                ProductCode = "AE004",
                IsAvailable = true
            },
            new DomainProduct
            {
                Id = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Title = "Ring Spotlight Cam",
                CreatedAt = new DateTime(2024, 6, 5, 13, 20, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 5, 13, 20, 0, DateTimeKind.Utc),
                Description =
                    "The Ring Spotlight Cam is a wireless security camera that lets you see, hear, and speak to anyone on your property from your phone, tablet, or PC. It has built-in spotlights and a siren to deter intruders, and it works with Alexa to let you control it with your voice.",
                ProductCode = "RS001",
                IsAvailable = true
            }
        );
    }
}