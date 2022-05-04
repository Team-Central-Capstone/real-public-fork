using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class userbodytypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserBodyTypeId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserBodyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBodyTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserBodyTypeId",
                table: "Users",
                column: "UserBodyTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserBodyTypes_UserBodyTypeId",
                table: "Users",
                column: "UserBodyTypeId",
                principalTable: "UserBodyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserBodyTypes_UserBodyTypeId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserBodyTypes");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserBodyTypeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserBodyTypeId",
                table: "Users");
        }
    }
}
