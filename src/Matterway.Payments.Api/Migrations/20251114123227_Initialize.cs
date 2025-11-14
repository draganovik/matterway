using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Matterway.Payments.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "text", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentAmount = table.Column<double>(type: "double precision", nullable: false),
                    CardNumber = table.Column<string>(type: "text", nullable: false),
                    CardHolder = table.Column<string>(type: "text", nullable: false),
                    ExpirationDate = table.Column<string>(type: "text", nullable: false),
                    SecurityCode = table.Column<string>(type: "text", nullable: false),
                    PaymentState = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "Id", "CardHolder", "CardNumber", "ExpirationDate", "PaymentAmount", "PaymentDate", "PaymentState", "ReferenceNumber", "SecurityCode" },
                values: new object[,]
                {
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"), "Mara Jakov", "1234-5678-1234-5678", "12/26", 39998.0, new DateTime(2024, 6, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, "5655-6666-7877", "1234" },
                    { new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"), "Stefan Stefanov", "8856-5678-1234-3366", "06/24", 4999.0, new DateTime(2024, 6, 2, 11, 30, 0, 0, DateTimeKind.Utc), 1, "6666-8888-6588", "6658" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");
        }
    }
}
