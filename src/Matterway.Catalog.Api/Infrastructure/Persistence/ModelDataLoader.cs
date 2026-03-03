using Matterway.Catalog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Catalog.Api.Infrastructure.Persistence;

public static class ModelDataLoader
{
    public static void InitializeDemo(ModelBuilder modelBuilder)
    {
        #region Articles data

        modelBuilder.Entity<Article>().HasData(
            new Article
            {
                Id = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Title = "Philips Hue White and Color Ambiance A19 Smart LED Bulb",
                CreatedAt = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                Description =
                    "The Philips Hue White and Color Ambiance A19 Smart LED Bulb lets you control your lights from your smartphone or tablet. Choose from 16 million colors to match the mood of any room, and set the lights to turn on and off on a schedule or when you're away from home.",
                ArticleCode = "PH002",
                IsAvailable = true,
                BasePrice = 4999
            },
            new Article
            {
                Id = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                CreatedAt = new DateTime(2024, 6, 2, 14, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 2, 14, 30, 0, DateTimeKind.Utc),
                Title = "Nest Learning Thermostat",
                Description =
                    "The 3rd generation Nest Learning Thermostat programs itself and automatically saves energy when you're away. It learns what temperature you like and builds a schedule around yours.",
                ArticleCode = "NT003",
                IsAvailable = true,
                BasePrice = 24999
            },
            new Article
            {
                Id = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                Title = "August Wi-Fi Smart Lock Pro",
                CreatedAt = new DateTime(2024, 6, 3, 16, 45, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 3, 16, 45, 0, DateTimeKind.Utc),
                Description =
                    "The August Wi-Fi Smart Lock Pro + Connect lets you add secure keyless entry to your home. Lock and unlock your door with your phone, and give keyless entry to family, friends, housekeepers, and other home services without worrying about lost or copied keys.",
                ArticleCode = "AL001",
                IsAvailable = true,
                BasePrice = 27999
            },
            new Article
            {
                Id = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                Title = "Amazon Echo (4th Gen)",
                CreatedAt = new DateTime(2024, 6, 4, 11, 15, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 4, 11, 15, 0, DateTimeKind.Utc),
                Description =
                    "The Amazon Echo (4th Gen) is a hands-free smart speaker that you control with your voice. It connects to Alexa to play music, make calls, set alarms and timers, ask questions, control smart home devices, and more.",
                ArticleCode = "AE004",
                IsAvailable = true,
                BasePrice = 9999
            },
            new Article
            {
                Id = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                Title = "Ring Spotlight Cam",
                CreatedAt = new DateTime(2024, 6, 5, 13, 20, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 6, 5, 13, 20, 0, DateTimeKind.Utc),
                Description =
                    "The Ring Spotlight Cam is a wireless security camera that lets you see, hear, and speak to anyone on your property from your phone, tablet, or PC. It has built-in spotlights and a siren to deter intruders, and it works with Alexa to let you control it with your voice.",
                ArticleCode = "RS001",
                IsAvailable = true,
                BasePrice = 19999
            }
        );

        #endregion

        #region Discounts data

        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Code = "WINTER25",
                ArticleId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                Percentage = 0.25m,
                ValidFrom = new DateTime(2025, 11, 19, 23, 0, 0, DateTimeKind.Utc),
                ValidTo = new DateTime(2026, 3, 19, 23, 0, 0, DateTimeKind.Utc)
            });

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
            new Detail { Slug = "video-quality", Title = "Video Quality" },
            new Detail { Slug = "battery-size", Title = "Battery Size", Unit = "mAh" },
            new Detail { Slug = "depth", Title = "Depth", Unit = "millimeters" },
            new Detail { Slug = "height", Title = "Height", Unit = "millimeters" },
            new Detail { Slug = "power", Title = "Power", Unit = "watts" },
            new Detail { Slug = "ram-size", Title = "RAM Size", Unit = "GB" },
            new Detail { Slug = "refresh-rate", Title = "Refresh Rate", Unit = "Hz" },
            new Detail { Slug = "screen-size", Title = "Screen Size", Unit = "inches" },
            new Detail { Slug = "storage", Title = "Storage", Unit = "GB" },
            new Detail { Slug = "weight", Title = "Weight", Unit = "grams" },
            new Detail { Slug = "width", Title = "Width", Unit = "millimeters" }
        );

        #endregion

        #region ArticleDetailTexts data

        modelBuilder.Entity<ArticleDetailText>().HasData(
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Apple HomeKit"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "display",
                Value = "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Siri"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "battery",
                Value = "Uses four AA batteries (included), lasts up to 6 months depending on usage"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "color-temperature",
                Value = "Adjustable from warm white (2700K) to daylight (6500K)"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "compatibility",
                Value = "Works with Alexa, Google Assistant, and Samsung SmartThings"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Bluetooth"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "video-quality",
                Value = "1080p HD"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "audio-quality",
                Value = "Two-way audio with noise cancellation"
            },
            new ArticleDetailText
            {
                ArticleId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "connectivity",
                Value = "Wi-Fi and Ethernet"
            }
        );

        #endregion

        #region ArticleDetailNumerics data

        modelBuilder.Entity<ArticleDetailNumeric>().HasData(
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "power",
                Value = 24m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "width",
                Value = 84m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "height",
                Value = 84m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "depth",
                Value = 28m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"),
                DetailSlug = "screen-size",
                Value = 2.0m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "battery-size",
                Value = 3000m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("0d6a9017-47e1-4477-86a9-67d9d9e468b8"),
                DetailSlug = "weight",
                Value = 400m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "power",
                Value = 9m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("a301b154-9867-431f-a9c9-0328b2ce350f"),
                DetailSlug = "weight",
                Value = 72m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                DetailSlug = "weight",
                Value = 970m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"),
                DetailSlug = "power",
                Value = 15m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "power",
                Value = 8m
            },
            new ArticleDetailNumeric
            {
                ArticleId = Guid.Parse("853cb7f2-bd31-4627-9da5-17b32cc8c157"),
                DetailSlug = "weight",
                Value = 480m
            }
        );

        #endregion
    }
}