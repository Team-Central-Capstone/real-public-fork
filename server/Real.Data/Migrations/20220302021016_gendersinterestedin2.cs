using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class gendersinterestedin2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "UserAttractedGenders",
                columns: table => new
                {
                    GendersAttractedToId = table.Column<int>(type: "int", nullable: false),
                    UserAttractedToId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAttractedGenders", x => new { x.GendersAttractedToId, x.UserAttractedToId });
                    table.ForeignKey(
                        name: "FK_UserAttractedGenders_UserGenders_GendersAttractedToId",
                        column: x => x.GendersAttractedToId,
                        principalTable: "UserGenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAttractedGenders_Users_UserAttractedToId",
                        column: x => x.UserAttractedToId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserAttractedGenders_UserAttractedToId",
                table: "UserAttractedGenders",
                column: "UserAttractedToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAttractedGenders");

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
    }
}
