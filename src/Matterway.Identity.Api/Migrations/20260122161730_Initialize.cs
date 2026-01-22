using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matterway.Identity.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                column: "ConcurrencyStamp",
                value: "758bc4a4-58db-4224-9851-bde84bcb01c7");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                column: "ConcurrencyStamp",
                value: "a5a2a12e-e523-46bd-8810-f0a5ad84e3fa");
        }
    }
}
