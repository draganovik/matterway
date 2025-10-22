using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageAlt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => new { x.Id, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "CreatedAt", "Description", "IsAvailable", "Price", "ProductCode", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), new DateTime(2024, 6, 3, 16, 45, 0, 0, DateTimeKind.Unspecified), "The August Wi-Fi Smart Lock Pro + Connect lets you add secure keyless entry to your home. Lock and unlock your door with your phone, and give keyless entry to family, friends, housekeepers, and other home services without worrying about lost or copied keys.", true, 27999.0, "AL-001", "August Wi-Fi Smart Lock Pro", new DateTime(2024, 6, 3, 16, 45, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), new DateTime(2024, 6, 4, 11, 15, 0, 0, DateTimeKind.Unspecified), "The Amazon Echo (4th Gen) is a hands-free smart speaker that you control with your voice. It connects to Alexa to play music, make calls, set alarms and timers, ask questions, control smart home devices, and more.", true, 9999.0, "AE-004", "Amazon Echo (4th Gen)", new DateTime(2024, 6, 4, 11, 15, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), new DateTime(2024, 6, 5, 13, 20, 0, 0, DateTimeKind.Unspecified), "The Ring Spotlight Cam is a wireless security camera that lets you see, hear, and speak to anyone on your property from your phone, tablet, or PC. It has built-in spotlights and a siren to deter intruders, and it works with Alexa to let you control it with your voice.", true, 19999.0, "RS-001", "Ring Spotlight Cam", new DateTime(2024, 6, 5, 13, 20, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Unspecified), "The 3rd generation Nest Learning Thermostat programs itself and automatically saves energy when you're away. It learns what temperature you like and builds a schedule around yours.", true, 24999.0, "NT-003", "Nest Learning Thermostat", new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), new DateTime(2024, 6, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "The Philips Hue White and Color Ambiance A19 Smart LED Bulb lets you control your lights from your smartphone or tablet. Choose from 16 million colors to match the mood of any room, and set the lights to turn on and off on a schedule or when you're away from home.", true, 4999.0, "PH-002", "Philips Hue White and Color Ambiance A19 Smart LED Bulb", new DateTime(2024, 6, 1, 9, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "ProductDetail",
                columns: new[] { "Id", "ProductId", "Title", "Type", "Unit", "Value" },
                values: new object[,]
                {
                    { new Guid("0a108c0c-d5b5-4486-90a6-0e7eb8d25a3c"), new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "Audio", 1, null, "Two-way audio with noise cancellation" },
                    { new Guid("0b80edc9-5351-4d75-9c12-7f53c15e74b8"), new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "Battery", 1, null, "Uses four AA batteries (included), lasts up to 6 months depending on usage" },
                    { new Guid("259bdf85-efb1-42e7-a8d1-9c7a6b71979a"), new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "Connectivity", 1, null, "Wi-Fi and Ethernet" },
                    { new Guid("30ef1d9a-13f8-4c2a-a2c3-5e5a5c23f5e1"), new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "Display", 1, null, "24-bit color LCD, 480 x 480 resolution at 229 pixels per inch (PPI)" },
                    { new Guid("3692d929-1534-4d4d-aae9-ec9e757b77c5"), new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "Color Temperature", 1, null, "Adjustable from warm white (2700K) to daylight (6500K)" },
                    { new Guid("5b5eaa60-3fb6-44f6-8640-bc56a55c986f"), new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "Weight", 1, "gram", "970" },
                    { new Guid("a46a6ea7-1e2c-427d-91f8-3d020b34d09c"), new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "Compatibility", 1, null, "Works with Alexa, Google Assistant, and Samsung SmartThings" },
                    { new Guid("ab0e76b4-69ea-4cc4-8cc2-f1d672dc2e2d"), new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "Compatibility", 1, null, "Works with Alexa, Google Assistant, and Siri" },
                    { new Guid("ae18a00e-7f3b-4df3-8d4c-df4a0b271a87"), new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "Connectivity", 1, null, "Wi-Fi and Bluetooth" },
                    { new Guid("b15e8e32-f357-4f97-9d19-1d2668e6d31a"), new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "Power", 1, null, "Requires 24VAC power, uses less than 1 kWh/month" },
                    { new Guid("eb69b58c-8f2d-48ee-b3eb-49a9d0ce49cb"), new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "Connectivity", 1, null, "Wi-Fi and Bluetooth" },
                    { new Guid("f5e5f5c5-5bf5-4c20-8b2d-f2f719e78508"), new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "Power", 1, "Watt", "9" },
                    { new Guid("f69c6d88-3a1c-46e8-bbcf-16d274f63052"), new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "Video", 1, null, "1080p HD" },
                    { new Guid("fd6f8de6-91c6-4362-ae90-6d8cf1d98f27"), new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "Compatibility", 1, null, "Works with Alexa, Google Assistant, and Apple HomeKit" }
                });

            migrationBuilder.InsertData(
                table: "ProductImage",
                columns: new[] { "Id", "ProductId", "ImageAlt", "ImageUrl", "IsMain" },
                values: new object[,]
                {
                    { 0, new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8"), "August Wi-Fi Smart Lock Pro - Front View", "https://images.homedepot-static.com/productImages/e2f3a648-f053-4e00-92fb-4349a0f344a2/svn/august-electronic-deadbolts-augsl05-m01-s01-64_1000.jpg", false },
                    { 0, new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c"), "Amazon Echo Show 5", "https://m.media-amazon.com/images/I/51iobpaEM5S._AC_SL1000_.jpg", false },
                    { 0, new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "Spotlight Cam Plus", "https://cdn.shopify.com/s/files/1/2393/8647/products/ring_spotlight_cam_plus_insitu_battery_1500x1500_0a5ecca0-fa41-49d7-86ad-01d797694845.jpg", false },
                    { 0, new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a"), "Nest Learning Thermostat - Front View", "https://i.pinimg.com/originals/95/99/16/959916d70bd67c4a5a3d160078b7f266.jpg", false },
                    { 0, new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "Philips Hue White and Color Ambiance A19 Smart LED Bulb - Front View", "https://images.homedepot-static.com/productImages/7d8edcf4-11b5-4cf1-8747-7ba637f618d1/svn/philips-led-bulbs-464487-64_1000.jpg", false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetail_ProductId",
                table: "ProductDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDetail");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
