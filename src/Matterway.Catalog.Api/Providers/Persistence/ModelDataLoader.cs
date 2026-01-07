using Microsoft.EntityFrameworkCore;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Providers.Persistence;

public static class ModelDataLoader
{
    public static void InitializeDemo(ModelBuilder modelBuilder)
    {
        #region Products data

        modelBuilder.Entity<Product>().HasData(
            new Product
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
            new Product
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
            new Product
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
            new Product
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
            new Product
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

        #endregion

        #region Prices data

        modelBuilder.Entity<Price>().HasData(
            new Price
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Currency = ESupportedCurrency.RSD,
                Amount = 4999
            },
            new Price
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                Currency = ESupportedCurrency.RSD,
                Amount = 24999
            },
            new Price
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Currency = ESupportedCurrency.RSD,
                Amount = 27999
            },
            new Price
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Currency = ESupportedCurrency.RSD,
                Amount = 9999
            },
            new Price
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Currency = ESupportedCurrency.RSD,
                Amount = 19999
            }
        );

        #endregion

        #region Discounts data

        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Code = "WINTER25",
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Currency = ESupportedCurrency.RSD,
                Percentage = 0.25m,
                ValidFrom = new DateTime(2025, 11, 20).ToUniversalTime(),
                ValidTo = new DateTime(2026, 3, 20).ToUniversalTime()
            });

        #endregion

        #region ProductImages data

        modelBuilder.Entity<ProductImage>().HasData(
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

        #endregion

        #region AttributeSlug data

        modelBuilder.Entity<AttributeSlug>().HasData(
            // Details
            new AttributeSlug { Slug = "audio" },
            new AttributeSlug { Slug = "audio-quality" },
            new AttributeSlug { Slug = "battery" },
            new AttributeSlug { Slug = "brand" },
            new AttributeSlug { Slug = "camera" },
            new AttributeSlug { Slug = "color" },
            new AttributeSlug { Slug = "color-temperature" },
            new AttributeSlug { Slug = "compatibility" },
            new AttributeSlug { Slug = "connectivity" },
            new AttributeSlug { Slug = "display" },
            new AttributeSlug { Slug = "features" },
            new AttributeSlug { Slug = "material" },
            new AttributeSlug { Slug = "model" },
            new AttributeSlug { Slug = "operating-system" },
            new AttributeSlug { Slug = "ports" },
            new AttributeSlug { Slug = "processor" },
            new AttributeSlug { Slug = "resolution" },
            new AttributeSlug { Slug = "video-quality" },
            // Specifications
            new AttributeSlug { Slug = "battery-size" },
            new AttributeSlug { Slug = "depth" },
            new AttributeSlug { Slug = "height" },
            new AttributeSlug { Slug = "power" },
            new AttributeSlug { Slug = "ram-size" },
            new AttributeSlug { Slug = "refresh-rate" },
            new AttributeSlug { Slug = "screen-size" },
            new AttributeSlug { Slug = "storage" },
            new AttributeSlug { Slug = "weight" },
            new AttributeSlug { Slug = "width" }
        );

        #endregion

        #region Details data

        modelBuilder.Entity<Detail>().HasData(
            new Detail { Slug = "audio", Title = "Audio" },
            new Detail { Slug = "audio-quality", Title = "Audio Quality" },
            new Detail { Slug = "battery", Title = "Battery" },
            new Detail { Slug = "brand", Title = "Brand" },
            new Detail { Slug = "camera", Title = "Camera" },
            new Detail { Slug = "color", Title = "Color" },
            new Detail { Slug = "color-temperature", Title = "Color Temperature" },
            new Detail { Slug = "compatibility", Title = "Compatibility" },
            new Detail { Slug = "connectivity", Title = "Connectivity" },
            new Detail { Slug = "display", Title = "Display" },
            new Detail { Slug = "features", Title = "Features" },
            new Detail { Slug = "material", Title = "Material" },
            new Detail { Slug = "model", Title = "Model" },
            new Detail { Slug = "operating-system", Title = "Operating System" },
            new Detail { Slug = "ports", Title = "Ports" },
            new Detail { Slug = "processor", Title = "Processor" },
            new Detail { Slug = "resolution", Title = "Resolution" },
            new Detail { Slug = "video-quality", Title = "Video Quality" }
        );

        #endregion

        #region ProductDetails data

        modelBuilder.Entity<ProductDetail>().HasData(
            new ProductDetail
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Apple HomeKit"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "display",
                Value = "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Siri"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "battery",
                Value = "Uses four AA batteries (included), lasts up to 6 months depending on usage"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "color-temperature",
                Value = "Adjustable from warm white (2700K) to daylight (6500K)"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Samsung SmartThings"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "video-quality",
                Value = "1080p HD"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "audio-quality",
                Value = "Two-way audio with noise cancellation"
            },
            new ProductDetail
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Ethernet"
            }
        );

        #endregion

        #region Specifications data

        modelBuilder.Entity<Specification>().HasData(
            new Specification { Slug = "battery-size", Title = "Battery Size", Unit = "mAh" },
            new Specification { Slug = "depth", Title = "Depth", Unit = "millimeters" },
            new Specification { Slug = "height", Title = "Height", Unit = "millimeters" },
            new Specification { Slug = "power", Title = "Power", Unit = "watts" },
            new Specification { Slug = "ram-size", Title = "RAM Size", Unit = "GB" },
            new Specification { Slug = "refresh-rate", Title = "Refresh Rate", Unit = "Hz" },
            new Specification { Slug = "screen-size", Title = "Screen Size", Unit = "inches" },
            new Specification { Slug = "storage", Title = "Storage", Unit = "GB" },
            new Specification { Slug = "weight", Title = "Weight", Unit = "grams" },
            new Specification { Slug = "width", Title = "Width", Unit = "millimeters" }
        );

        #endregion

        #region ProductSpecifications data

        modelBuilder.Entity<ProductSpecification>().HasData(
            new ProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "power",
                Value = 24m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "width",
                Value = 84m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "height",
                Value = 84m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "depth",
                Value = 28m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                SpecificationSlug = "screen-size",
                Value = 2.0m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                SpecificationSlug = "battery-size",
                Value = 3000m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                SpecificationSlug = "weight",
                Value = 400m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                SpecificationSlug = "power",
                Value = 9m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                SpecificationSlug = "weight",
                Value = 72m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                SpecificationSlug = "weight",
                Value = 970m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                SpecificationSlug = "power",
                Value = 15m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                SpecificationSlug = "power",
                Value = 8m
            },
            new ProductSpecification
            {
                ProductId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                SpecificationSlug = "weight",
                Value = 480m
            }
        );

        #endregion
    }
}