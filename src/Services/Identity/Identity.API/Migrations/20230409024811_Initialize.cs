using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                values: new object[] { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2023, 4, 9, 2, 48, 11, 272, DateTimeKind.Utc).AddTicks(7341), "user@example.com", "AQAAAAIAAYagAAAAENpALCvs+2eF8D3WNtiCky5pLNgQaonBmUxiNKbuqOudWb3VS8wQVbXpcBvs5cTS2w==", 0 });

            migrationBuilder.InsertData(
                table: "Session",
                columns: new[] { "Id", "Created", "Expires", "RefreshExpires", "RefreshToken", "SystemUserId", "Token" },
                values: new object[] { new Guid("4e54e945-90e7-4f75-88f7-9d9b84d7c81c"), new DateTime(2023, 4, 9, 2, 48, 11, 328, DateTimeKind.Utc).AddTicks(5286), new DateTime(2023, 4, 9, 3, 3, 11, 328, DateTimeKind.Utc).AddTicks(5290), new DateTime(2023, 4, 21, 2, 48, 11, 328, DateTimeKind.Utc).AddTicks(6502), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2ODEwMDg0OTEsImV4cCI6MTY4MjA0NTI5MSwiaWF0IjoxNjgxMDA4NDkxfQ.aFmfTCgqzuirTUDNNLLH_LfI4gAhATPRrenF9zQtAaY", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2ODEwMDg0OTEsImV4cCI6MTY4MTAwOTM5MSwiaWF0IjoxNjgxMDA4NDkxfQ.a3T2WSCmFRzEZ4-WZ7yWo1s6kOjdvUQbpGa0dYU0VLM" });

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
