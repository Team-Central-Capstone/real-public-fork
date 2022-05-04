using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class matches_additionalcolumns2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RawMatchPercentage",
                table: "UserMatches",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WeightedMatchPercentage",
                table: "UserMatches",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawMatchPercentage",
                table: "UserMatches");

            migrationBuilder.DropColumn(
                name: "WeightedMatchPercentage",
                table: "UserMatches");
        }
    }
}
