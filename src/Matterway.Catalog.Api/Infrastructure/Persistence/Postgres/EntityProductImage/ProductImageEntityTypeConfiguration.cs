using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Catalog.Api.Infrastructure.Persistence.Postgres.EntityProductImage;

internal sealed class ProductImageEntityTypeConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable(nameof(ProductImage));

        builder.HasKey(pi => new { pi.Id, pi.ProductId });

        builder.Property(pi => pi.OrderIndex)
            .IsRequired();

        builder.Property(pi => pi.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(pi => pi.ImageAlt)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(pi => new { pi.ProductId, pi.OrderIndex }).IsUnique();

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages!)
            .HasForeignKey(pi => pi.ProductId);

        builder.HasData(
            new ProductImage
            {
                Id = Guid.Parse("9dc0c1db-a949-4cb8-8a8c-2f55de2f1f90"),
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                OrderIndex = 0,
                ImageAlt = "Amazon Echo Show 5",
                ImageUrl = "https://m.media-amazon.com/images/I/51iobpaEM5S._AC_SL1000_.jpg"
            },
            new ProductImage
            {
                Id = Guid.Parse("05ffe3d2-d56d-4fd2-b816-7b1ef82b1e62"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                OrderIndex = 0,
                ImageAlt = "Spotlight Cam Plus",
                ImageUrl =
                    "https://images.ctfassets.net/a3peezndovsu/product-24529407541337-media/6eaa58ced96b0dc6959181f55dec6023/product-24529407541337-media.jpg"
            },
            new ProductImage
            {
                Id = Guid.Parse("72cebb50-7f20-4c2a-9803-ccc9934274be"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                OrderIndex = 0,
                ImageAlt = "Philips Hue White and Color Ambiance A19 Smart LED Bulb - Front View",
                ImageUrl =
                    "https://images.homedepot-static.com/productImages/7d8edcf4-11b5-4cf1-8747-7ba637f618d1/svn/philips-led-bulbs-464487-64_1000.jpg"
            },
            new ProductImage
            {
                Id = Guid.Parse("55ab96f9-8b3b-42b0-a933-643522cd7397"),
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                OrderIndex = 0,
                ImageAlt = "Nest Learning Thermostat - Front View",
                ImageUrl = "https://i.pinimg.com/originals/95/99/16/959916d70bd67c4a5a3d160078b7f266.jpg"
            },
            new ProductImage
            {
                Id = Guid.Parse("88423aa2-93bb-462c-9934-7e783e680b98"),
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                OrderIndex = 0,
                ImageAlt = "August Wi-Fi Smart Lock Pro - Front View",
                ImageUrl =
                    "https://images.homedepot-static.com/productImages/e2f3a648-f053-4e00-92fb-4349a0f344a2/svn/august-electronic-deadbolts-augsl05-m01-s01-64_1000.jpg"
            }
        );
    }
}