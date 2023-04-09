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
            values: new object[] { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2023, 4, 9, 0, 44, 6, 17, DateTimeKind.Utc).AddTicks(4950), "user@example.com", "AQAAAAIAAYagAAAAEAVgTGCN7tL6L8P2EdfHshGrb2B3yVVvEYc6cF/bYAXOwZrTOl01EIpvNbTw3ihz2A==", 0 });

        migrationBuilder.InsertData(
            table: "Session",
            columns: new[] { "Id", "Created", "Expires", "RefreshExpires", "RefreshToken", "SystemUserId", "Token" },
            values: new object[] { new Guid("4e54e945-90e7-4f75-88f7-9d9b84d7c81c"), new DateTime(2023, 4, 9, 0, 44, 6, 73, DateTimeKind.Utc).AddTicks(3737), new DateTime(2023, 4, 9, 0, 59, 6, 73, DateTimeKind.Utc).AddTicks(3741), new DateTime(2023, 4, 21, 0, 44, 6, 73, DateTimeKind.Utc).AddTicks(4909), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2ODEwMDEwNDYsImV4cCI6MTY4MjAzNzg0NiwiaWF0IjoxNjgxMDAxMDQ2fQ.sj2ij6VUd7vQtnG6v6E6dDyEmBNpMce0W-mnPTk6QrE", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJhOWQ2NGI2NC05M2MxLTQxYTgtYTc0Mi04YThiYTgxZTIwYjEiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE2ODEwMDEwNDYsImV4cCI6MTY4MTAwMTk0NiwiaWF0IjoxNjgxMDAxMDQ2fQ.gsboHa4CdJ_z_e8B-FFuy0uWXaPbuAg_Y8oihs8Gs5Q" });

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
