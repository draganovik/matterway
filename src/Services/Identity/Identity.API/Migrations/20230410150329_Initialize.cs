using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.API.Migrations;

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
            values: new object[] { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2023, 4, 10, 15, 3, 29, 391, DateTimeKind.Utc).AddTicks(2303), "user@example.com", "AQAAAAIAAYagAAAAEAlare2oJP0vS3Fg4YXY7aW3y9dY8tYCM0AZaeDhTyvbavRc/6pOgYWe6S32OxZelQ==", 0 });

        migrationBuilder.InsertData(
            table: "Session",
            columns: new[] { "Id", "Created", "Expires", "RefreshExpires", "RefreshToken", "SystemUserId", "Token" },
            values: new object[] { new Guid("4e54e945-90e7-4f75-88f7-9d9b84d7c81c"), new DateTime(2023, 4, 10, 15, 3, 29, 448, DateTimeKind.Utc).AddTicks(3882), new DateTime(2023, 4, 10, 15, 18, 29, 448, DateTimeKind.Utc).AddTicks(3889), new DateTime(2023, 4, 22, 15, 3, 29, 448, DateTimeKind.Utc).AddTicks(5368), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2ODExMzkwMDksImV4cCI6MTY4MjE3NTgwOSwiaWF0IjoxNjgxMTM5MDA5fQ.GzroQO36mJm9I80czlxd0Qisi7c44If1XJCzjiOk9LY", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2ODExMzkwMDksImV4cCI6MTY4MTEzOTkwOSwiaWF0IjoxNjgxMTM5MDA5fQ.T0M-fQvuncXmkCD-O7CJt0GlZp4D8o-jiRCbAHFivlM" });

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
