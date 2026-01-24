using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("6f055a46-6bf0-4fdb-8c27-1877a2b6f811"), "f9f0b98d-7d9e-48d8-86c5-9a13ebf1c81a", "Employee", "EMPLOYEE" },
                    { new Guid("c4c29ba9-3b22-416f-8a37-8a7c3d6ed1f9"), "a40b1c8b-7f9f-4cbb-9de8-8c79d9f0dd8e", "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Created", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"), 0, "3d5f0a44-e0f6-4b13-8a06-1dc8db48fe74", new DateTime(2021, 9, 9, 10, 10, 10, 0, DateTimeKind.Utc), "mladen@matterway.local", false, false, null, "MLADEN@MATTERWAY.LOCAL", "MLADEN@MATTERWAY.LOCAL", "AQAAAAIAAYagAAAAEF0PUBp9R90/+4Ul8J4HdcOJNNv1Ol5W2vIK2ooqOFRTc1vcgJtxBVqWYlONUSjEJw==", null, false, "4a1f3895-9229-45c9-9fcb-7b8f2a9f46b3", false, "mladen@matterway.local" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2"), 0, "cb6a7ad6-7c4c-4bb6-bce5-8aa4d1bf7b58", new DateTime(2022, 10, 10, 11, 11, 11, 0, DateTimeKind.Utc), "jelena@matterway.local", false, false, null, "JELENA@MATTERWAY.LOCAL", "JELENA@MATTERWAY.LOCAL", "AQAAAAIAAYagAAAAEJru7jHMkz3Cdn5rk9dTj3umC8lMLv0XfkY1k5PXeuh2VkwiazogHTe8IoEhYWr6rg==", null, false, "bf0cb1b8-d485-4fdd-8a89-fb70120dbd9f", false, "jelena@matterway.local" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), 0, "371c6716-8af4-48b7-ac6b-61120c592477", new DateTime(2023, 4, 12, 12, 10, 0, 0, DateTimeKind.Utc), "stefan999@mail.local", false, false, null, "STEFAN999@MAIL.LOCAL", "STEFAN999@MAIL.LOCAL", "AQAAAAIAAYagAAAAEFSn2wwtXMTkX5P1GglolnIwSI7iauwvIgZXlrTih7oTJ/FWJkY/CXmdNCR4ZJKAug==", null, false, "019bada3-b4dc-7155-baca-576dd9914d57", false, "stefan999@mail.local" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), 0, "3b9f3e46-3c3c-4f04-9b6f-4fe90f0a8c80", new DateTime(2024, 2, 20, 9, 30, 0, 0, DateTimeKind.Utc), "marag@mail.local", false, false, null, "MARAG@MAIL.LOCAL", "MARAG@MAIL.LOCAL", "AQAAAAIAAYagAAAAEEPDW3c2GRf473Cn5iK6pW8vBqQuPvei43fnIK+WbTaixjrzkd/mMyXGIp2+IoDI8Q==", null, false, "8ca7016d-0e4d-4b92-96cb-c964d7d8f9c1", false, "marag@mail.local" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[,]
                {
                    { 1, "perm", "identity:administrator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1") },
                    { 2, "perm", "catalog:administrator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1") },
                    { 3, "perm", "customers:administrator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1") },
                    { 4, "perm", "sales:administrator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1") },
                    { 5, "perm", "identity:operator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2") },
                    { 6, "perm", "catalog:operator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2") },
                    { 7, "perm", "customers:operator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2") },
                    { 8, "perm", "sales:operator", new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2") }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("6f055a46-6bf0-4fdb-8c27-1877a2b6f811"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1") },
                    { new Guid("6f055a46-6bf0-4fdb-8c27-1877a2b6f811"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2") },
                    { new Guid("c4c29ba9-3b22-416f-8a37-8a7c3d6ed1f9"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3") },
                    { new Guid("c4c29ba9-3b22-416f-8a37-8a7c3d6ed1f9"), new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
