using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Matterway.Ordering.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverName = table.Column<string>(type: "text", nullable: false),
                    Residence = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeliveryAddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Address_DeliveryAddressId",
                        column: x => x.DeliveryAddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderStatus = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderHistory_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<double>(type: "double precision", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "City", "Country", "Note", "ReceiverName", "Residence", "Street", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), "Sremska Mitrovica", "Serbia", null, "Mara Jakov", "54", "Njegoševa", "22000" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), "Novi Sad", "Serbia", null, "Stefan Stefanov", "3", "Narodnih Heroja", "21000" }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "Id", "CustomerId", "DeliveryAddressId", "ReferenceNumber" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), "6666-8888-6588" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), "5655-6666-7877" }
                });

            migrationBuilder.InsertData(
                table: "OrderHistory",
                columns: new[] { "Id", "CreatedDate", "Description", "OrderId", "OrderStatus" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"), new DateTime(2024, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Order Ready", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"), 1 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b8"), new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Utc), "Order Canceled", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"), 5 }
                });

            migrationBuilder.InsertData(
                table: "OrderItem",
                columns: new[] { "OrderId", "ProductId", "ProductName", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"), new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "Philips Hue White and Color Ambiance A19 Smart LED Bulb", 1, 4999.0 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"), new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "Ring Spotlight Cam", 2, 19999.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryAddressId",
                table: "Order",
                column: "DeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHistory_OrderId",
                table: "OrderHistory",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderHistory");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
