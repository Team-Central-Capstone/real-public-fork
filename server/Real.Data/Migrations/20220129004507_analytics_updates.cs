using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class analytics_updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Analytics");

            migrationBuilder.RenameColumn(
                name: "IPAddress",
                table: "Analytics",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Controller",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FirebaseUserId",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IPv4",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IPv6",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Namespace",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "QueryString",
                table: "Analytics",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "Controller",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "FirebaseUserId",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "Host",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "IPv4",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "IPv6",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "Namespace",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Analytics");

            migrationBuilder.DropColumn(
                name: "QueryString",
                table: "Analytics");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Analytics",
                newName: "IPAddress");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Analytics",
                type: "int",
                nullable: true);
        }
    }
}
