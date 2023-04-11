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
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), new DateTime(2023, 4, 11, 20, 22, 9, 292, DateTimeKind.Local).AddTicks(1647), "mladen@matterway.com", "AQAAAAIAAYagAAAAEPzJrgrSHvlWMNcuRY61SIa97LzzESCMd9z/c3U3JJpwW78pYAS8CzFxwf/T4mj3kw==", 0 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2"), new DateTime(2023, 4, 11, 20, 22, 9, 348, DateTimeKind.Local).AddTicks(858), "jelena@matterway.com", "AQAAAAIAAYagAAAAEIDx+M7HXM1Kl2zzUb8FiuoDP7fBbBMZ/p70CdaqyxtZYWt6JPor9OkTytpBkQeyyw==", 1 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), new DateTime(2023, 4, 11, 20, 22, 9, 404, DateTimeKind.Local).AddTicks(3301), "stefan999@gmail.com", "AQAAAAIAAYagAAAAENE0Fpv9DGGx4FOw04QfzvZ2EG07+UscItavmJLtZsQ83H7dZrW7lk1ZJK0UAPgH6g==", 2 },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), new DateTime(2023, 4, 11, 20, 22, 9, 459, DateTimeKind.Local).AddTicks(4763), "marag2@gmail.com", "AQAAAAIAAYagAAAAEACFx7d5JHJmkOKu0q2Bp60MKQlM7EGiis3GjvkTm2qNYWeSjcBWtaGe0zxapYEP5w==", 2 }
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
