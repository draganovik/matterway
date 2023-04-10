using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.API.Migrations;

/// <inheritdoc />
public partial class Initialize : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Address",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Residence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Address", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Order",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DeliveryAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Order", x => x.Id);
                table.ForeignKey(
                    name: "FK_Order_Address_DeliveryAddressId",
                    column: x => x.DeliveryAddressId,
                    principalTable: "Address",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderHistory",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrderStatus = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderHistory", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderHistory_Order_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Order",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderItem",
            columns: table => new
            {
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UnitPrice = table.Column<double>(type: "float", nullable: false),
                Units = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItem", x => new { x.OrderId, x.ProductId });
                table.ForeignKey(
                    name: "FK_OrderItem_Order_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Order",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Order_DeliveryAddressId",
            table: "Order",
            column: "DeliveryAddressId");

        migrationBuilder.CreateIndex(
            name: "IX_OrderHistory_OrderId",
            table: "OrderHistory",
            column: "OrderId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OrderHistory");

        migrationBuilder.DropTable(
            name: "OrderItem");

        migrationBuilder.DropTable(
            name: "Order");

        migrationBuilder.DropTable(
            name: "Address");
    }
}
