using Microsoft.EntityFrameworkCore.Migrations;

namespace Hasebni.SqlServer.Migrations
{
    public partial class doubletoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AmountAdded",
                table: "Notifications",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "AmountAdded",
                table: "Notifications",
                type: "float",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
