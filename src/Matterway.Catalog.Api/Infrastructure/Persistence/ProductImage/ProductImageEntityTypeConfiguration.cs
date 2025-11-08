using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainProductImage = Matterway.Catalog.Api.Domain.ProductImage;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;

internal sealed class ProductImageEntityTypeConfiguration : IEntityTypeConfiguration<DomainProductImage>
{
    public void Configure(EntityTypeBuilder<DomainProductImage> builder)
    {
        builder.ToTable(nameof(DomainProductImage));

        builder.HasKey(pi => new { pi.Id, pi.ProductId });

        builder.Property(pi => pi.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pi => pi.ImageRef)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(pi => pi.ImageAlt)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(pi => pi.ImageRef).IsUnique();

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages!)
            .HasForeignKey(pi => pi.ProductId);

        builder.HasData(
            new DomainProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                ImageAlt = "Amazon Echo Show 5",
                ImageRef = "seed-20d76c1a-6d4e-4f22-9262-c20dc62f6f2c-0",
                ImageUrl = "https://m.media-amazon.com/images/I/51iobpaEM5S._AC_SL1000_.jpg"
            },
            new DomainProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                ImageAlt = "Spotlight Cam Plus",
                ImageRef = "seed-853cb7f2-bd31-4627-9da5-17b32cc8c157-0",
                ImageUrl =
                    "https://cdn.shopify.com/s/files/1/2393/8647/products/ring_spotlight_cam_plus_insitu_battery_1500x1500_0a5ecca0-fa41-49d7-86ad-01d797694845.jpg"
            },
            new DomainProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                ImageAlt = "Philips Hue White and Color Ambiance A19 Smart LED Bulb - Front View",
                ImageRef = "seed-a301b154-9867-431f-a9c9-0328b2ce350f-0",
                ImageUrl =
                    "https://images.homedepot-static.com/productImages/7d8edcf4-11b5-4cf1-8747-7ba637f618d1/svn/philips-led-bulbs-464487-64_1000.jpg"
            },
            new DomainProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                ImageAlt = "Nest Learning Thermostat - Front View",
                ImageRef = "seed-8d9d9e68-6d44-49c7-8fcb-a6db28969e5a-0",
                ImageUrl = "https://i.pinimg.com/originals/95/99/16/959916d70bd67c4a5a3d160078b7f266.jpg"
            },
            new DomainProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                ImageAlt = "August Wi-Fi Smart Lock Pro - Front View",
                ImageRef = "seed-0d6a9017-47e1-4477-86a9-67d9d9e468b8-0",
                ImageUrl =
                    "https://images.homedepot-static.com/productImages/e2f3a648-f053-4e00-92fb-4349a0f344a2/svn/august-electronic-deadbolts-augsl05-m01-s01-64_1000.jpg"
            }
        );
    }
}