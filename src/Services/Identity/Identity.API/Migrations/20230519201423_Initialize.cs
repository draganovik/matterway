using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.API.Migrations
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshExpires = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2023, 5, 19, 22, 14, 23, 462, DateTimeKind.Local).AddTicks(4361), "mladen@matterway.com", "AQAAAAIAAYagAAAAEEqMsFNFhWyMQXEN0rB6Mp4cFGB5GmXQgH2ZCqOkmm0oGKVRSWgbFy47iIEP++tMGA==", 0 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2"), new DateTime(2023, 5, 19, 22, 14, 23, 525, DateTimeKind.Local).AddTicks(5091), "jelena@matterway.com", "AQAAAAIAAYagAAAAEJ5gC972Ja2WkjhcvANz+CoARvsoWoO4zUQyq27YOvD6jKx1nyv91Ks1YuhHeV65RA==", 1 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), new DateTime(2023, 5, 19, 22, 14, 23, 598, DateTimeKind.Local).AddTicks(9885), "stefan999@gmail.com", "AQAAAAIAAYagAAAAEO5rXg/F4umM8Ij0848OWgklsQeGJiANCcElu+/hJjOulHtbTDlmQGNnVharzfAyPA==", 2 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new DateTime(2023, 5, 19, 22, 14, 23, 692, DateTimeKind.Local).AddTicks(9253), "marag2@gmail.com", "AQAAAAIAAYagAAAAENpnbURk+8JmVcACvRb4P/hDHnPd4xkjo3BfGzR/GEFuJeCJ1uzmzMtstphFHh42GA==", 2 }
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
