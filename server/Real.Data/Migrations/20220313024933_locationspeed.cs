using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class locationspeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RollingAverageSpeed",
                table: "Locations",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpeedFromLast",
                table: "Locations",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RollingAverageSpeed",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "SpeedFromLast",
                table: "Locations");
        }
    }
}
