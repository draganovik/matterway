using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                name: "DomainProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainProduct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainProductDetailType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainProductDetailType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainProductImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ImageAlt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainProductImage", x => new { x.Id, x.ProductId });
                    table.ForeignKey(
                        name: "FK_DomainProductImage_DomainProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "DomainProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DomainProductDetail",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainProductDetail", x => new { x.ProductId, x.TypeId });
                    table.ForeignKey(
                        name: "FK_DomainProductDetail_DomainProductDetailType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DomainProductDetailType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DomainProductDetail_DomainProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "DomainProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DomainProduct",
                columns: new[] { "Id", "CreatedAt", "Description", "IsAvailable", "Price", "ProductCode", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), new DateTime(2024, 6, 3, 16, 45, 0, 0, DateTimeKind.Utc), "The August Wi-Fi Smart Lock Pro + Connect lets you add secure keyless entry to your home. Lock and unlock your door with your phone, and give keyless entry to family, friends, housekeepers, and other home services without worrying about lost or copied keys.", true, 27999.0, "AL-001", "August Wi-Fi Smart Lock Pro", new DateTime(2024, 6, 3, 16, 45, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), new DateTime(2024, 6, 4, 11, 15, 0, 0, DateTimeKind.Utc), "The Amazon Echo (4th Gen) is a hands-free smart speaker that you control with your voice. It connects to Alexa to play music, make calls, set alarms and timers, ask questions, control smart home devices, and more.", true, 9999.0, "AE-004", "Amazon Echo (4th Gen)", new DateTime(2024, 6, 4, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), new DateTime(2024, 6, 5, 13, 20, 0, 0, DateTimeKind.Utc), "The Ring Spotlight Cam is a wireless security camera that lets you see, hear, and speak to anyone on your property from your phone, tablet, or PC. It has built-in spotlights and a siren to deter intruders, and it works with Alexa to let you control it with your voice.", true, 19999.0, "RS-001", "Ring Spotlight Cam", new DateTime(2024, 6, 5, 13, 20, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Utc), "The 3rd generation Nest Learning Thermostat programs itself and automatically saves energy when you're away. It learns what temperature you like and builds a schedule around yours.", true, 24999.0, "NT-003", "Nest Learning Thermostat", new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), new DateTime(2024, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc), "The Philips Hue White and Color Ambiance A19 Smart LED Bulb lets you control your lights from your smartphone or tablet. Choose from 16 million colors to match the mood of any room, and set the lights to turn on and off on a schedule or when you're away from home.", true, 4999.0, "PH-002", "Philips Hue White and Color Ambiance A19 Smart LED Bulb", new DateTime(2024, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "DomainProductDetailType",
                columns: new[] { "Id", "Title", "Unit" },
                values: new object[,]
                {
                    { 1, "Width", "millimeters" },
                    { 2, "Height", "millimeters" },
                    { 3, "Depth", "millimeters" },
                    { 4, "Weight", "grams" },
                    { 5, "Color", null },
                    { 6, "Material", null },
                    { 7, "Connectivity", null },
                    { 8, "Power", "Watts" },
                    { 9, "Battery", null },
                    { 10, "Compatibility", null },
                    { 11, "Connectivity", null },
                    { 12, "Display", null },
                    { 13, "Color Temperature", "Kelvin" },
                    { 14, "Video Quality", null },
                    { 15, "Audio Quality", null }
                });

            migrationBuilder.InsertData(
                table: "DomainProductDetail",
                columns: new[] { "ProductId", "TypeId", "Value" },
                values: new object[,]
                {
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), 9, "Uses four AA batteries (included), lasts up to 6 months depending on usage" },
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), 10, "Works with Alexa, Google Assistant, and Siri" },
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), 11, "Wi-Fi and Bluetooth" },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), 4, "970" },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), 7, "Wi-Fi and Bluetooth" },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), 11, "Wi-Fi and Ethernet" },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), 14, "1080p HD" },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), 15, "Two-way audio with noise cancellation" },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), 7, "Works with Alexa, Google Assistant, and Apple HomeKit" },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), 8, "Requires 24VAC power, uses less than 1 kWh/month" },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), 12, "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)" },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), 8, "9" },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), 10, "Works with Alexa, Google Assistant, and Samsung SmartThings" },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), 13, "Adjustable from warm white (2700K) to daylight (6500K)" }
                });

            migrationBuilder.InsertData(
                table: "DomainProductImage",
                columns: new[] { "Id", "ProductId", "ImageAlt", "ImageUrl", "OrderIndex" },
                values: new object[,]
                {
                    { new Guid("05ffe3d2-d56d-4fd2-b816-7b1ef82b1e62"), new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "Spotlight Cam Plus", "https://cdn.shopify.com/s/files/1/2393/8647/products/ring_spotlight_cam_plus_insitu_battery_1500x1500_0a5ecca0-fa41-49d7-86ad-01d797694845.jpg", 0 },
                    { new Guid("55ab96f9-8b3b-42b0-a933-643522cd7397"), new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "Nest Learning Thermostat - Front View", "https://i.pinimg.com/originals/95/99/16/959916d70bd67c4a5a3d160078b7f266.jpg", 0 },
                    { new Guid("72cebb50-7f20-4c2a-9803-ccc9934274be"), new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "Philips Hue White and Color Ambiance A19 Smart LED Bulb - Front View", "https://images.homedepot-static.com/productImages/7d8edcf4-11b5-4cf1-8747-7ba637f618d1/svn/philips-led-bulbs-464487-64_1000.jpg", 0 },
                    { new Guid("88423aa2-93bb-462c-9934-7e783e680b98"), new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "August Wi-Fi Smart Lock Pro - Front View", "https://images.homedepot-static.com/productImages/e2f3a648-f053-4e00-92fb-4349a0f344a2/svn/august-electronic-deadbolts-augsl05-m01-s01-64_1000.jpg", 0 },
                    { new Guid("9dc0c1db-a949-4cb8-8a8c-2f55de2f1f90"), new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "Amazon Echo Show 5", "https://m.media-amazon.com/images/I/51iobpaEM5S._AC_SL1000_.jpg", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainProductDetail_TypeId",
                table: "DomainProductDetail",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainProductImage_ProductId_OrderIndex",
                table: "DomainProductImage",
                columns: new[] { "ProductId", "OrderIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainProductDetail");

            migrationBuilder.DropTable(
                name: "DomainProductImage");

            migrationBuilder.DropTable(
                name: "DomainProductDetailType");

            migrationBuilder.DropTable(
                name: "DomainProduct");
        }
    }
}
