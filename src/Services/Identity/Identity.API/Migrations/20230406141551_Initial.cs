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
                values: new object[] { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2023, 4, 6, 14, 15, 51, 718, DateTimeKind.Utc).AddTicks(417), "user@example.com", "AQAAAAIAAYagAAAAEA1kLvCNFQ0vjv+Cu2Xjn8U8cmdpaPcVZme1NbBwi+BqsPHknc8eZqgNKR1gwsPXWQ==", 2 });

            migrationBuilder.InsertData(
                table: "Session",
                columns: new[] { "Id", "Created", "Expires", "RefreshExpires", "RefreshToken", "SystemUserId", "Token" },
                values: new object[] { new Guid("4e54e945-90e7-4f75-88f7-9d9b84d7c81c"), new DateTime(2023, 4, 6, 14, 15, 51, 779, DateTimeKind.Utc).AddTicks(3595), new DateTime(2023, 4, 6, 14, 30, 51, 779, DateTimeKind.Utc).AddTicks(3601), new DateTime(2023, 4, 18, 14, 15, 51, 779, DateTimeKind.Utc).AddTicks(4976), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQ3VzdG9tZXIiLCJuYmYiOjE2ODA3OTA1NTEsImV4cCI6MTY4MTgyNzM1MSwiaWF0IjoxNjgwNzkwNTUxfQ.qFUI_kKexjjryYGolpX-nTB0P7OIJCsUXZyUPRg4HkU", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQ3VzdG9tZXIiLCJuYmYiOjE2ODA3OTA1NTEsImV4cCI6MTY4MDc5MTQ1MSwiaWF0IjoxNjgwNzkwNTUxfQ.QTZO_JjZ_Eo6mMeMAanWGCOh-eIIGmhISXAN9f7H4wo" });

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
