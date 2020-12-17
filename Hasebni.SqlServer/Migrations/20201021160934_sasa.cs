using Microsoft.EntityFrameworkCore.Migrations;

namespace Hasebni.SqlServer.Migrations
{
    public partial class sasa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Profiles_ProfileId",
                table: "Device");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Device",
                table: "Device");

            migrationBuilder.RenameTable(
                name: "Device",
                newName: "Devices");

            migrationBuilder.RenameIndex(
                name: "IX_Device_ProfileId",
                table: "Devices",
                newName: "IX_Devices_ProfileId");

            migrationBuilder.AlterColumn<long>(
                name: "Balance",
                table: "Members",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Devices",
                table: "Devices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Profiles_ProfileId",
                table: "Devices",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Profiles_ProfileId",
                table: "Devices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Devices",
                table: "Devices");

            migrationBuilder.RenameTable(
                name: "Devices",
                newName: "Device");

            migrationBuilder.RenameIndex(
                name: "IX_Devices_ProfileId",
                table: "Device",
                newName: "IX_Device_ProfileId");

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "Members",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Device",
                table: "Device",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Profiles_ProfileId",
                table: "Device",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
