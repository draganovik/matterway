using Catalog.Api.Features.ProductDetails.Domain;
using Catalog.Api.Features.ProductImages.Domain;
using Catalog.Api.Features.Products.Domain;
using Common.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductDetail> ProductDetail { get; set; } = default!;

    public DbSet<ProductImage> ProductImage { get; set; } = default!;

    public DbSet<Product> Product { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Title = "Philips Hue White and Color Ambiance A19 Smart LED Bulb",
                CreatedAt = DateTime.Parse("2024-06-01T09:00:00"),
                UpdatedAt = DateTime.Parse("2024-06-01T09:00:00"),
                Description =
                    "The Philips Hue White and Color Ambiance A19 Smart LED Bulb lets you control your lights from your smartphone or tablet. Choose from 16 million colors to match the mood of any room, and set the lights to turn on and off on a schedule or when you're away from home.",
                Price = 4999,
                ProductCode = "PH-002",
                IsAvailable = true
            },
            new Product
            {
                Id = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                CreatedAt = DateTime.Parse("2024-06-02T14:30:00"),
                UpdatedAt = DateTime.Parse("2024-06-02T14:30:00"),
                Title = "Nest Learning Thermostat",
                Description =
                    "The 3rd generation Nest Learning Thermostat programs itself and automatically saves energy when you're away. It learns what temperature you like and builds a schedule around yours.",
                Price = 24999,
                ProductCode = "NT-003",
                IsAvailable = true
            },
            new Product
            {
                Id = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Title = "August Wi-Fi Smart Lock Pro",
                CreatedAt = DateTime.Parse("2024-06-03T16:45:00"),
                UpdatedAt = DateTime.Parse("2024-06-03T16:45:00"),
                Description =
                    "The August Wi-Fi Smart Lock Pro + Connect lets you add secure keyless entry to your home. Lock and unlock your door with your phone, and give keyless entry to family, friends, housekeepers, and other home services without worrying about lost or copied keys.",
                Price = 27999,
                ProductCode = "AL-001",
                IsAvailable = true
            },
            new Product
            {
                Id = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Title = "Amazon Echo (4th Gen)",
                CreatedAt = DateTime.Parse("2024-06-04T11:15:00"),
                UpdatedAt = DateTime.Parse("2024-06-04T11:15:00"),
                Description =
                    "The Amazon Echo (4th Gen) is a hands-free smart speaker that you control with your voice. It connects to Alexa to play music, make calls, set alarms and timers, ask questions, control smart home devices, and more.",
                Price = 9999,
                ProductCode = "AE-004",
                IsAvailable = true
            },
            new Product
            {
                Id = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Title = "Ring Spotlight Cam",
                CreatedAt = DateTime.Parse("2024-06-05T13:20:00"),
                UpdatedAt = DateTime.Parse("2024-06-05T13:20:00"),
                Description =
                    "The Ring Spotlight Cam is a wireless security camera that lets you see, hear, and speak to anyone on your property from your phone, tablet, or PC. It has built-in spotlights and a siren to deter intruders, and it works with Alexa to let you control it with your voice.",
                Price = 19999,
                ProductCode = "RS-001",
                IsAvailable = true
            }
        );
        modelBuilder.Entity<ProductDetail>().HasData(
            new ProductDetail
            {
                Id = Guid.Parse("fd6f8de6-91c6-4362-ae90-6d8cf1d98f27"),
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Type = DetailType.Specification,
                Title = "Compatibility",
                Value = "Works with Alexa, Google Assistant, and Apple HomeKit"
            },
            new ProductDetail
            {
                Id = Guid.Parse("30ef1d9a-13f8-4c2a-a2c3-5e5a5c23f5e1"),
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Type = DetailType.Specification,
                Title = "Display",
                Value = "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)"
            },
            new ProductDetail
            {
                Id = Guid.Parse("b15e8e32-f357-4f97-9d19-1d2668e6d31a"),
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Type = DetailType.Specification,
                Title = "Power",
                Value = "Requires 24VAC power, uses less than 1 kWh/month"
            },
            new ProductDetail
            {
                Id = Guid.Parse("eb69b58c-8f2d-48ee-b3eb-49a9d0ce49cb"),
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Type = DetailType.Specification,
                Title = "Connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ProductDetail
            {
                Id = Guid.Parse("ab0e76b4-69ea-4cc4-8cc2-f1d672dc2e2d"),
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Type = DetailType.Specification,
                Title = "Compatibility",
                Value = "Works with Alexa, Google Assistant, and Siri"
            },
            new ProductDetail
            {
                Id = Guid.Parse("0b80edc9-5351-4d75-9c12-7f53c15e74b8"),
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Type = DetailType.Specification,
                Title = "Battery",
                Value = "Uses four AA batteries (included), lasts up to 6 months depending on usage"
            },
            new ProductDetail
            {
                Id = Guid.Parse("3692d929-1534-4d4d-aae9-ec9e757b77c5"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Type = DetailType.Specification,
                Title = "Color Temperature",
                Value = "Adjustable from warm white (2700K) to daylight (6500K)"
            },
            new ProductDetail
            {
                Id = Guid.Parse("a46a6ea7-1e2c-427d-91f8-3d020b34d09c"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Type = DetailType.Specification,
                Title = "Compatibility",
                Value = "Works with Alexa, Google Assistant, and Samsung SmartThings"
            },
            new ProductDetail
            {
                Id = Guid.Parse("f5e5f5c5-5bf5-4c20-8b2d-f2f719e78508"),
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Type = DetailType.Specification,
                Title = "Power",
                Value = "9",
                Unit = "Watt"
            },
            new ProductDetail
            {
                Id = Guid.Parse("5b5eaa60-3fb6-44f6-8640-bc56a55c986f"),
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Type = DetailType.Specification,
                Title = "Weight",
                Value = "970",
                Unit = "gram"
            },
            new ProductDetail
            {
                Id = Guid.Parse("ae18a00e-7f3b-4df3-8d4c-df4a0b271a87"),
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Type = DetailType.Specification,
                Title = "Connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ProductDetail
            {
                Id = Guid.Parse("f69c6d88-3a1c-46e8-bbcf-16d274f63052"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Type = DetailType.Specification,
                Title = "Video",
                Value = "1080p HD"
            },
            new ProductDetail
            {
                Id = Guid.Parse("0a108c0c-d5b5-4486-90a6-0e7eb8d25a3c"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Type = DetailType.Specification,
                Title = "Audio",
                Value = "Two-way audio with noise cancellation"
            },
            new ProductDetail
            {
                Id = Guid.Parse("259bdf85-efb1-42e7-a8d1-9c7a6b71979a"),
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Type = DetailType.Specification,
                Title = "Connectivity",
                Value = "Wi-Fi and Ethernet"
            }
        );
        modelBuilder.Entity<ProductImage>().HasData(
            new ProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                ImageAlt = "Amazon Echo Show 5",
                ImageUrl = "https://m.media-amazon.com/images/I/51iobpaEM5S._AC_SL1000_.jpg"
            },
            new ProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                ImageAlt = "Spotlight Cam Plus",
                ImageUrl =
                    "https://cdn.shopify.com/s/files/1/2393/8647/products/ring_spotlight_cam_plus_insitu_battery_1500x1500_0a5ecca0-fa41-49d7-86ad-01d797694845.jpg"
            },
            new ProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                ImageAlt = "Philips Hue White and Color Ambiance A19 Smart LED Bulb - Front View",
                ImageUrl =
                    "https://images.homedepot-static.com/productImages/7d8edcf4-11b5-4cf1-8747-7ba637f618d1/svn/philips-led-bulbs-464487-64_1000.jpg"
            },
            new ProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                ImageAlt = "Nest Learning Thermostat - Front View",
                ImageUrl = "https://i.pinimg.com/originals/95/99/16/959916d70bd67c4a5a3d160078b7f266.jpg"
            },
            new ProductImage
            {
                Id = 0,
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                ImageAlt = "August Wi-Fi Smart Lock Pro - Front View",
                ImageUrl =
                    "https://images.homedepot-static.com/productImages/e2f3a648-f053-4e00-92fb-4349a0f344a2/svn/august-electronic-deadbolts-augsl05-m01-s01-64_1000.jpg"
            }
        );
    }
}