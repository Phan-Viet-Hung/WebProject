using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Ban_Hang.Migrations
{
    public partial class u : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillDetails_Bill_ordersId",
                table: "BillDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BillDetails_Products_productId",
                table: "BillDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_CartItems_CartItemsId",
                table: "CartDetails");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartDetails_CartItemsId",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "CartItemsId",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "ItemsId",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "orderId",
                table: "BillDetails");

            migrationBuilder.RenameColumn(
                name: "productId",
                table: "BillDetails",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "ordersId",
                table: "BillDetails",
                newName: "BillId");

            migrationBuilder.RenameIndex(
                name: "IX_BillDetails_productId",
                table: "BillDetails",
                newName: "IX_BillDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_BillDetails_ordersId",
                table: "BillDetails",
                newName: "IX_BillDetails_BillId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Carts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BillDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "BillDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_BillItems_OrderId",
                table: "BillItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BillItems_ProductId",
                table: "BillItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_UserId",
                table: "Bill",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_Users_UserId",
                table: "Bill",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BillDetails_Bill_BillId",
                table: "BillDetails",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BillDetails_Products_ProductId",
                table: "BillDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillItems_Bill_OrderId",
                table: "BillItems",
                column: "OrderId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BillItems_Products_ProductId",
                table: "BillItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_Users_UserId",
                table: "Bill");

            migrationBuilder.DropForeignKey(
                name: "FK_BillDetails_Bill_BillId",
                table: "BillDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BillDetails_Products_ProductId",
                table: "BillDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BillItems_Bill_OrderId",
                table: "BillItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BillItems_Products_ProductId",
                table: "BillItems");

            migrationBuilder.DropIndex(
                name: "IX_BillItems_OrderId",
                table: "BillItems");

            migrationBuilder.DropIndex(
                name: "IX_BillItems_ProductId",
                table: "BillItems");

            migrationBuilder.DropIndex(
                name: "IX_Bill_UserId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BillDetails");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "BillDetails");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "BillDetails",
                newName: "productId");

            migrationBuilder.RenameColumn(
                name: "BillId",
                table: "BillDetails",
                newName: "ordersId");

            migrationBuilder.RenameIndex(
                name: "IX_BillDetails_ProductId",
                table: "BillDetails",
                newName: "IX_BillDetails_productId");

            migrationBuilder.RenameIndex(
                name: "IX_BillDetails_BillId",
                table: "BillDetails",
                newName: "IX_BillDetails_ordersId");

            migrationBuilder.AlterColumn<string>(
                name: "ProductName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "CartItemsId",
                table: "CartDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ItemsId",
                table: "CartDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "orderId",
                table: "BillDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartItemsId",
                table: "CartDetails",
                column: "CartItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillDetails_Bill_ordersId",
                table: "BillDetails",
                column: "ordersId",
                principalTable: "Bill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BillDetails_Products_productId",
                table: "BillDetails",
                column: "productId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_CartItems_CartItemsId",
                table: "CartDetails",
                column: "CartItemsId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
