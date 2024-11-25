using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Ban_Hang.Migrations
{
    public partial class qqq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Carts",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "Bill",
                newName: "CreateDate");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Carts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Carts",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Bill",
                newName: "OrderDate");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
