using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Matterway.Customers.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DefaultAddressId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => new { x.CustomerId, x.ProductId });
                    table.CheckConstraint("CK_CartItem_Quantity", "\"Quantity\" >= 1");
                    table.ForeignKey(
                        name: "FK_CartItem_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "BirthDate", "DefaultAddressId", "FirstName", "LastName", "SystemUserId" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), new DateOnly(1980, 1, 1), null, "Stefan", "Stefanov", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3") },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new DateOnly(2000, 5, 5), null, "Mara", "Jakov", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4") }
                });

            migrationBuilder.InsertData(
                table: "CartItem",
                columns: new[] { "CustomerId", "ProductId", "ProductName", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157"), "Ring Spotlight Cam", 1, 19999m },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new Guid("a301b154-9867-431f-a9c9-0328b2ce350f"), "Philips Hue White and Color Ambiance A19 Smart LED Bulb", 3, 4999m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CustomerId",
                table: "CartItem",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_SystemUserId",
                table: "Customer",
                column: "SystemUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
