using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Matterway.Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeSlug",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeSlug", x => x.Slug);
                });

            migrationBuilder.CreateTable(
                name: "ArticleImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ImageAlt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleImage", x => new { x.Id, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_ArticleImage_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => new { x.Code, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_Discount_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Detail",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detail", x => x.Slug);
                    table.ForeignKey(
                        name: "FK_Detail_AttributeSlug_Slug",
                        column: x => x.Slug,
                        principalTable: "AttributeSlug",
                        principalColumn: "Slug",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Specification",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specification", x => x.Slug);
                    table.ForeignKey(
                        name: "FK_Specification_AttributeSlug_Slug",
                        column: x => x.Slug,
                        principalTable: "AttributeSlug",
                        principalColumn: "Slug",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticleDetail",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetailSlug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleDetail", x => new { x.ArticleId, x.DetailSlug });
                    table.ForeignKey(
                        name: "FK_ArticleDetail_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleDetail_Detail_DetailSlug",
                        column: x => x.DetailSlug,
                        principalTable: "Detail",
                        principalColumn: "Slug",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleSpecification",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecificationSlug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleSpecification", x => new { x.ArticleId, x.SpecificationSlug });
                    table.ForeignKey(
                        name: "FK_ArticleSpecification_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleSpecification_Specification_SpecificationSlug",
                        column: x => x.SpecificationSlug,
                        principalTable: "Specification",
                        principalColumn: "Slug",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Article",
                columns: new[] { "Id", "ArticleCode", "BasePrice", "CreatedAt", "Description", "IsAvailable", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "AL001", 27999m, new DateTime(2024, 6, 3, 16, 45, 0, 0, DateTimeKind.Utc), "The August Wi-Fi Smart Lock Pro + Connect lets you add secure keyless entry to your home. Lock and unlock your door with your phone, and give keyless entry to family, friends, housekeepers, and other home services without worrying about lost or copied keys.", true, "August Wi-Fi Smart Lock Pro", new DateTime(2024, 6, 3, 16, 45, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "AE004", 9999m, new DateTime(2024, 6, 4, 11, 15, 0, 0, DateTimeKind.Utc), "The Amazon Echo (4th Gen) is a hands-free smart speaker that you control with your voice. It connects to Alexa to play music, make calls, set alarms and timers, ask questions, control smart home devices, and more.", true, "Amazon Echo (4th Gen)", new DateTime(2024, 6, 4, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "RS001", 19999m, new DateTime(2024, 6, 5, 13, 20, 0, 0, DateTimeKind.Utc), "The Ring Spotlight Cam is a wireless security camera that lets you see, hear, and speak to anyone on your property from your phone, tablet, or PC. It has built-in spotlights and a siren to deter intruders, and it works with Alexa to let you control it with your voice.", true, "Ring Spotlight Cam", new DateTime(2024, 6, 5, 13, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "NT003", 24999m, new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Utc), "The 3rd generation Nest Learning Thermostat programs itself and automatically saves energy when you're away. It learns what temperature you like and builds a schedule around yours.", true, "Nest Learning Thermostat", new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "PH002", 4999m, new DateTime(2024, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc), "The Philips Hue White and Color Ambiance A19 Smart LED Bulb lets you control your lights from your smartphone or tablet. Choose from 16 million colors to match the mood of any room, and set the lights to turn on and off on a schedule or when you're away from home.", true, "Philips Hue White and Color Ambiance A19 Smart LED Bulb", new DateTime(2024, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AttributeSlug",
                column: "Slug",
                values: new object[]
                {
                    "audio",
                    "audio-quality",
                    "battery",
                    "battery-size",
                    "brand",
                    "camera",
                    "color",
                    "color-temperature",
                    "compatibility",
                    "connectivity",
                    "depth",
                    "display",
                    "features",
                    "height",
                    "material",
                    "model",
                    "operating-system",
                    "ports",
                    "power",
                    "processor",
                    "ram-size",
                    "refresh-rate",
                    "resolution",
                    "screen-size",
                    "storage",
                    "video-quality",
                    "weight",
                    "width"
                });

            migrationBuilder.InsertData(
                table: "ArticleImage",
                columns: new[] { "ArticleId", "Id", "ImageAlt", "ImageUrl", "OrderIndex" },
                values: new object[,]
                {
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), new Guid("05ffe3d2-d56d-4fd2-b816-7b1ef82b1e62"), "Spotlight Cam Plus", "https://images.ctfassets.net/a3peezndovsu/article-24529407541337-media/6eaa58ced96b0dc6959181f55dec6023/article-24529407541337-media.jpg", 0 },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), new Guid("55ab96f9-8b3b-42b0-a933-643522cd7397"), "Nest Learning Thermostat - Front View", "https://i.pinimg.com/originals/95/99/16/959916d70bd67c4a5a3d160078b7f266.jpg", 0 },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), new Guid("72cebb50-7f20-4c2a-9803-ccc9934274be"), "Philips Hue White and Color Ambiance A19 Smart LED Bulb - Front View", "https://images.homedepot-static.com/articleImages/7d8edcf4-11b5-4cf1-8747-7ba637f618d1/svn/philips-led-bulbs-464487-64_1000.jpg", 0 },
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), new Guid("88423aa2-93bb-462c-9934-7e783e680b98"), "August Wi-Fi Smart Lock Pro - Front View", "https://images.homedepot-static.com/articleImages/e2f3a648-f053-4e00-92fb-4349a0f344a2/svn/august-electronic-deadbolts-augsl05-m01-s01-64_1000.jpg", 0 },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), new Guid("9dc0c1db-a949-4cb8-8a8c-2f55de2f1f90"), "Amazon Echo Show 5", "https://m.media-amazon.com/images/I/51iobpaEM5S._AC_SL1000_.jpg", 0 }
                });

            migrationBuilder.InsertData(
                table: "Detail",
                columns: new[] { "Slug", "Title" },
                values: new object[,]
                {
                    { "audio", "Audio" },
                    { "audio-quality", "Audio Quality" },
                    { "battery", "Battery" },
                    { "brand", "Brand" },
                    { "camera", "Camera" },
                    { "color", "Color" },
                    { "color-temperature", "Color Temperature" },
                    { "compatibility", "Compatibility" },
                    { "connectivity", "Connectivity" },
                    { "display", "Display" },
                    { "features", "Features" },
                    { "material", "Material" },
                    { "model", "Model" },
                    { "operating-system", "Operating System" },
                    { "ports", "Ports" },
                    { "processor", "Processor" },
                    { "resolution", "Resolution" },
                    { "video-quality", "Video Quality" }
                });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "ArticleId", "Code", "Percentage", "ValidFrom", "ValidTo" },
                values: new object[] { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "WINTER25", 0.25m, new DateTime(2025, 11, 19, 23, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 19, 23, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Specification",
                columns: new[] { "Slug", "Title", "Unit" },
                values: new object[,]
                {
                    { "battery-size", "Battery Size", "mAh" },
                    { "depth", "Depth", "millimeters" },
                    { "height", "Height", "millimeters" },
                    { "power", "Power", "watts" },
                    { "ram-size", "RAM Size", "GB" },
                    { "refresh-rate", "Refresh Rate", "Hz" },
                    { "screen-size", "Screen Size", "inches" },
                    { "storage", "Storage", "GB" },
                    { "weight", "Weight", "grams" },
                    { "width", "Width", "millimeters" }
                });

            migrationBuilder.InsertData(
                table: "ArticleDetail",
                columns: new[] { "ArticleId", "DetailSlug", "Value" },
                values: new object[,]
                {
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "battery", "Uses four AA batteries (included), lasts up to 6 months depending on usage" },
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "compatibility", "Works with Alexa, Google Assistant, and Siri" },
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "connectivity", "Wi-Fi and Bluetooth" },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "connectivity", "Wi-Fi and Bluetooth" },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "audio-quality", "Two-way audio with noise cancellation" },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "connectivity", "Wi-Fi and Ethernet" },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "video-quality", "1080p HD" },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "compatibility", "Works with Alexa, Google Assistant, and Apple HomeKit" },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "display", "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)" },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "color-temperature", "Adjustable from warm white (2700K) to daylight (6500K)" },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "compatibility", "Works with Alexa, Google Assistant, and Samsung SmartThings" }
                });

            migrationBuilder.InsertData(
                table: "ArticleSpecification",
                columns: new[] { "ArticleId", "SpecificationSlug", "Value" },
                values: new object[,]
                {
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "battery-size", 3000m },
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "weight", 400m },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "power", 15m },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "weight", 970m },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "power", 8m },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "weight", 480m },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "depth", 28m },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "height", 84m },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "power", 24m },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "screen-size", 2.0m },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "width", 84m },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "power", 9m },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "weight", 72m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_ArticleCode",
                table: "Article",
                column: "ArticleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetail_DetailSlug",
                table: "ArticleDetail",
                column: "DetailSlug");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleImage_ArticleId_OrderIndex",
                table: "ArticleImage",
                columns: new[] { "ArticleId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleSpecification_SpecificationSlug",
                table: "ArticleSpecification",
                column: "SpecificationSlug");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSlug_Slug",
                table: "AttributeSlug",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discount_ArticleId",
                table: "Discount",
                column: "ArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleDetail");

            migrationBuilder.DropTable(
                name: "ArticleImage");

            migrationBuilder.DropTable(
                name: "ArticleSpecification");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Detail");

            migrationBuilder.DropTable(
                name: "Specification");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "AttributeSlug");
        }
    }
}
