using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class analytics3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Analytics",
                newName: "StartTimestamp");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTimestamp",
                table: "Analytics",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTimestamp",
                table: "Analytics");

            migrationBuilder.RenameColumn(
                name: "StartTimestamp",
                table: "Analytics",
                newName: "Timestamp");
        }
    }
}
