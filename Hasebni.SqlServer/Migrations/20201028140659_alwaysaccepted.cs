using Microsoft.EntityFrameworkCore.Migrations;

namespace Hasebni.SqlServer.Migrations
{
    public partial class alwaysaccepted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Notifications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAlwaysAccepted",
                table: "Members",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsAlwaysAccepted",
                table: "Members");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
