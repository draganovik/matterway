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
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerOrder",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlacedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOrder", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_CustomerOrder_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerArticle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    ArticleName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ArticleCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<string>(type: "character varying(19)", maxLength: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerArticle", x => x.Id);
                    table.CheckConstraint("CK_CustomerArticle_Quantity", "\"Quantity\" >= 1");
                    table.ForeignKey(
                        name: "FK_CustomerArticle_CustomerOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "CustomerOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerArticle_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "BirthDate", "DefaultAddressId", "FirstName", "LastName" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), new DateOnly(1980, 1, 1), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"), "Stefan", "Stefanov" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new DateOnly(2000, 5, 5), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"), "Mara", "Jakov" }
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "ContactPhone", "Country", "CustomerId", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"), "Futog", "23b", "Novi Sad", "+381601234567", "Serbia", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), "21000" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"), "Kralja Milana", "34/10", "Beograd", "+381676543210", "Serbia", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), "11000" }
                });

            migrationBuilder.InsertData(
                table: "CustomerArticle",
                columns: new[] { "Id", "ArticleCode", "ArticleName", "CustomerId", "OrderId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b7"), "PHUE0002", "Philips Hue White and Color Ambiance A19 Smart LED Bulb", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), null, 3, 4999m },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b8"), "RING0001", "Ring Spotlight Cam", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), null, 1, 19999m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CustomerId",
                table: "Address",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerArticle_CustomerId",
                table: "CustomerArticle",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerArticle_CustomerId_ArticleCode",
                table: "CustomerArticle",
                columns: new[] { "CustomerId", "ArticleCode" },
                unique: true,
                filter: "\"OrderId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerArticle_CustomerId_ArticleCode_OrderId",
                table: "CustomerArticle",
                columns: new[] { "CustomerId", "ArticleCode", "OrderId" },
                unique: true,
                filter: "\"OrderId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerArticle_OrderId",
                table: "CustomerArticle",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrder_CustomerId",
                table: "CustomerOrder",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "CustomerArticle");

            migrationBuilder.DropTable(
                name: "CustomerOrder");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
