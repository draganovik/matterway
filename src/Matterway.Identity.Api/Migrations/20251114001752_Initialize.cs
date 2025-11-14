using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Matterway.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshExpires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Session_SystemUser_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SystemUser",
                columns: new[] { "Id", "Created", "Email", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2021, 9, 9, 10, 10, 10, 0, DateTimeKind.Utc), "mladen@matterway.local", "AQAAAAIAAYagAAAAEF0PUBp9R90/+4Ul8J4HdcOJNNv1Ol5W2vIK2ooqOFRTc1vcgJtxBVqWYlONUSjEJw==", 0 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2"), new DateTime(2022, 10, 10, 11, 11, 11, 0, DateTimeKind.Utc), "jelena@matterway.local", "AQAAAAIAAYagAAAAEJru7jHMkz3Cdn5rk9dTj3umC8lMLv0XfkY1k5PXeuh2VkwiazogHTe8IoEhYWr6rg==", 1 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), new DateTime(2023, 4, 12, 12, 10, 0, 0, DateTimeKind.Utc), "stefan999@mail.local", "AQAAAAIAAYagAAAAEFSn2wwtXMTkX5P1GglolnIwSI7iauwvIgZXlrTih7oTJ/FWJkY/CXmdNCR4ZJKAug==", 2 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new DateTime(2024, 2, 20, 9, 30, 0, 0, DateTimeKind.Utc), "marag@mail.local", "AQAAAAIAAYagAAAAEEPDW3c2GRf473Cn5iK6pW8vBqQuPvei43fnIK+WbTaixjrzkd/mMyXGIp2+IoDI8Q==", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Session_SystemUserId",
                table: "Session",
                column: "SystemUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "SystemUser");
        }
    }
}
