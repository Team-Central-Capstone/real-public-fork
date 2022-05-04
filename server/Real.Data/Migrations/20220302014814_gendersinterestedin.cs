using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class gendersinterestedin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserGenders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGenders_UserId",
                table: "UserGenders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGenders_Users_UserId",
                table: "UserGenders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGenders_Users_UserId",
                table: "UserGenders");

            migrationBuilder.DropIndex(
                name: "IX_UserGenders_UserId",
                table: "UserGenders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserGenders");
        }
    }
}
