using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class precalc_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "Precalc_Locations",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //         FirebaseUserId = table.Column<string>(type: "longtext", nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         Latitude = table.Column<double>(type: "double", nullable: false),
            //         Longitude = table.Column<double>(type: "double", nullable: false),
            //         StartTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         EndTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         trace = table.Column<string>(type: "longtext", nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Precalc_Locations", x => x.Id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");

            // migrationBuilder.CreateTable(
            //     name: "Precalc_ProfileMatches",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //         lUserId = table.Column<int>(type: "int", nullable: false),
            //         rUserId = table.Column<int>(type: "int", nullable: false),
            //         TotalPossibleQuestions = table.Column<long>(type: "bigint", nullable: false),
            //         MatchedQuestions = table.Column<int>(type: "int", nullable: false),
            //         RawMatchPercentage = table.Column<double>(type: "double", nullable: false),
            //         WeightedMatchPercentage = table.Column<double>(type: "double", nullable: false),
            //         Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Precalc_ProfileMatches", x => x.Id);
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Precalc_Locations");

            migrationBuilder.DropTable(
                name: "Precalc_ProfileMatches");
        }
    }
}
