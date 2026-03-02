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
                name: "Detail",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detail", x => x.Slug);
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
                name: "ArticleDetailNumeric",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetailSlug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleDetailNumeric", x => new { x.ArticleId, x.DetailSlug });
                    table.ForeignKey(
                        name: "FK_ArticleDetailNumeric_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleDetailNumeric_Detail_DetailSlug",
                        column: x => x.DetailSlug,
                        principalTable: "Detail",
                        principalColumn: "Slug",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleDetailText",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetailSlug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleDetailText", x => new { x.ArticleId, x.DetailSlug });
                    table.ForeignKey(
                        name: "FK_ArticleDetailText_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleDetailText_Detail_DetailSlug",
                        column: x => x.DetailSlug,
                        principalTable: "Detail",
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
                table: "Detail",
                columns: new[] { "Slug", "Title", "Unit" },
                values: new object[,]
                {
                    { "audio", "Audio", null },
                    { "audio-quality", "Audio Quality", null },
                    { "battery", "Battery", null },
                    { "battery-size", "Battery Size", "mAh" },
                    { "brand", "Brand", null },
                    { "camera", "Camera", null },
                    { "color", "Color", null },
                    { "color-temperature", "Color Temperature", null },
                    { "compatibility", "Compatibility", null },
                    { "connectivity", "Connectivity", null },
                    { "depth", "Depth", "millimeters" },
                    { "display", "Display", null },
                    { "features", "Features", null },
                    { "height", "Height", "millimeters" },
                    { "material", "Material", null },
                    { "model", "Model", null },
                    { "operating-system", "Operating System", null },
                    { "ports", "Ports", null },
                    { "power", "Power", "watts" },
                    { "processor", "Processor", null },
                    { "ram-size", "RAM Size", "GB" },
                    { "refresh-rate", "Refresh Rate", "Hz" },
                    { "resolution", "Resolution", null },
                    { "screen-size", "Screen Size", "inches" },
                    { "storage", "Storage", "GB" },
                    { "video-quality", "Video Quality", null },
                    { "weight", "Weight", "grams" },
                    { "width", "Width", "millimeters" }
                });

            migrationBuilder.InsertData(
                table: "ArticleDetailNumeric",
                columns: new[] { "ArticleId", "DetailSlug", "Value" },
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

            migrationBuilder.InsertData(
                table: "ArticleDetailText",
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
                table: "Discount",
                columns: new[] { "ArticleId", "Code", "Percentage", "ValidFrom", "ValidTo" },
                values: new object[] { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "WINTER25", 0.25m, new DateTime(2025, 11, 19, 23, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 19, 23, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Article_ArticleCode",
                table: "Article",
                column: "ArticleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetailNumeric_DetailSlug",
                table: "ArticleDetailNumeric",
                column: "DetailSlug");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleDetailText_DetailSlug",
                table: "ArticleDetailText",
                column: "DetailSlug");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleImage_ArticleId_OrderIndex",
                table: "ArticleImage",
                columns: new[] { "ArticleId", "OrderIndex" },
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
                name: "ArticleDetailNumeric");

            migrationBuilder.DropTable(
                name: "ArticleDetailText");

            migrationBuilder.DropTable(
                name: "ArticleImage");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Detail");

            migrationBuilder.DropTable(
                name: "Article");
        }
    }
}
