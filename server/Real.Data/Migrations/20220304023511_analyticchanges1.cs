using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class analyticchanges1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalyticDetails_Analytics_AnalyticId",
                table: "AnalyticDetails");

            migrationBuilder.DropIndex(
                name: "IX_AnalyticDetails_AnalyticId",
                table: "AnalyticDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "AnalyticDetailId",
                table: "Analytics",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalyticDetails_Analytics_Id",
                table: "AnalyticDetails",
                column: "Id",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnalyticDetails_Analytics_Id",
                table: "AnalyticDetails");

            migrationBuilder.DropColumn(
                name: "AnalyticDetailId",
                table: "Analytics");

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticDetails_AnalyticId",
                table: "AnalyticDetails",
                column: "AnalyticId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnalyticDetails_Analytics_AnalyticId",
                table: "AnalyticDetails",
                column: "AnalyticId",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
