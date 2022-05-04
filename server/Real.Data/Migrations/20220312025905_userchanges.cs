using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class userchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeightInches",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfileIntro",
                table: "Users",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeightInches",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileIntro",
                table: "Users");
        }
    }
}
