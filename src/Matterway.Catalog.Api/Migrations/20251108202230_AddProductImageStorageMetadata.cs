using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Matterway.Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImageStorageMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageRef",
                table: "DomainProductImage",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "DomainProductImage",
                keyColumns: new[] { "Id", "ProductId" },
                keyValues: new object[] { 0, new Guid("0d6a9017-47e1-4477-86a9-67d9d9e468b8") },
                column: "ImageRef",
                value: "seed-0d6a9017-47e1-4477-86a9-67d9d9e468b8-0");

            migrationBuilder.UpdateData(
                table: "DomainProductImage",
                keyColumns: new[] { "Id", "ProductId" },
                keyValues: new object[] { 0, new Guid("20d76c1a-6d4e-4f22-9262-c20dc62f6f2c") },
                column: "ImageRef",
                value: "seed-20d76c1a-6d4e-4f22-9262-c20dc62f6f2c-0");

            migrationBuilder.UpdateData(
                table: "DomainProductImage",
                keyColumns: new[] { "Id", "ProductId" },
                keyValues: new object[] { 0, new Guid("853cb7f2-bd31-4627-9da5-17b32cc8c157") },
                column: "ImageRef",
                value: "seed-853cb7f2-bd31-4627-9da5-17b32cc8c157-0");

            migrationBuilder.UpdateData(
                table: "DomainProductImage",
                keyColumns: new[] { "Id", "ProductId" },
                keyValues: new object[] { 0, new Guid("8d9d9e68-6d44-49c7-8fcb-a6db28969e5a") },
                column: "ImageRef",
                value: "seed-8d9d9e68-6d44-49c7-8fcb-a6db28969e5a-0");

            migrationBuilder.UpdateData(
                table: "DomainProductImage",
                keyColumns: new[] { "Id", "ProductId" },
                keyValues: new object[] { 0, new Guid("a301b154-9867-431f-a9c9-0328b2ce350f") },
                column: "ImageRef",
                value: "seed-a301b154-9867-431f-a9c9-0328b2ce350f-0");

            migrationBuilder.CreateIndex(
                name: "IX_DomainProductImage_ImageRef",
                table: "DomainProductImage",
                column: "ImageRef",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DomainProductImage_ImageRef",
                table: "DomainProductImage");

            migrationBuilder.DropColumn(
                name: "ImageRef",
                table: "DomainProductImage");
        }
    }
}
