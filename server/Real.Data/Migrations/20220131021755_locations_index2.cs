using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class locations_index2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"Beginning migration at {DateTime.Now}");
            migrationBuilder.CreateIndex(
                name: "IX_Locations_FirebaseUserId",
                table: "Locations",
                column: "FirebaseUserId");
            Console.WriteLine($"Completed migration at {DateTime.Now}");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_FirebaseUserId",
                table: "Locations");
        }
    }
}
