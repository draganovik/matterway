using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                values: new object[] { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2023, 3, 31, 13, 18, 16, 39, DateTimeKind.Utc).AddTicks(1151), "user@example.com", "AQAAAAIAAYagAAAAEPJ7tY6J1UfLmM4vRQ3zg2wrKHnmhiJ5BoDogxS63HI7Nx916b+lp6m/setlZrTSYA==", 2 });

            migrationBuilder.InsertData(
                table: "Session",
                columns: new[] { "Id", "Created", "Expires", "RefreshExpires", "RefreshToken", "SystemUserId", "Token" },
                values: new object[] { new Guid("4e54e945-90e7-4f75-88f7-9d9b84d7c81c"), new DateTime(2023, 3, 31, 13, 18, 16, 104, DateTimeKind.Utc).AddTicks(781), new DateTime(2023, 3, 31, 13, 33, 16, 104, DateTimeKind.Utc).AddTicks(788), new DateTime(2023, 4, 12, 13, 18, 16, 104, DateTimeKind.Utc).AddTicks(2241), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQ3VzdG9tZXIiLCJuYmYiOjE2ODAyNjg2OTYsImV4cCI6MTY4MTMwNTQ5NiwiaWF0IjoxNjgwMjY4Njk2fQ.gIqBOvqHYkW9AQ6Hfrob6GhhSQbnI4AP_udGVcb977o", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQ3VzdG9tZXIiLCJuYmYiOjE2ODAyNjg2OTYsImV4cCI6MTY4MDI2OTU5NiwiaWF0IjoxNjgwMjY4Njk2fQ.lzK7qD6vKVIUXe9Ut1Lb4grHN_UqqfuBqtdlWzGQCVM" });

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
