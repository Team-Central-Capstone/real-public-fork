using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class QuestionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.RenameTable(
            //     name: "Capstone_Information_Schema",
            //     newName: "Capstone_Information_Schema_Columns");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "SurveyQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // migrationBuilder.CreateTable(
            //     name: "Capstone_Information_Schema_Tables",
            //     columns: table => new
            //     {
            //         TABLE_CATALOG = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         TABLE_SCHEMA = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         TABLE_NAME = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         TABLE_TYPE = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         ENGINE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         VERSION = table.Column<int>(type: "int", nullable: true),
            //         ROW_FORMAT = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         TABLE_ROWS = table.Column<long>(type: "bigint", nullable: true),
            //         AVG_ROW_LENGTH = table.Column<long>(type: "bigint", nullable: true),
            //         DATA_LENGTH = table.Column<long>(type: "bigint", nullable: true),
            //         MAX_DATA_LENGTH = table.Column<long>(type: "bigint", nullable: true),
            //         INDEX_LENGTH = table.Column<long>(type: "bigint", nullable: true),
            //         DATA_FREE = table.Column<long>(type: "bigint", nullable: true),
            //         AUTO_INCREMENT = table.Column<long>(type: "bigint", nullable: true),
            //         CREATE_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: false),
            //         UPDATE_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //         CHECK_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //         TABLE_COLLATION = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         CHECKSUM = table.Column<long>(type: "bigint", nullable: true),
            //         CREATE_OPTIONS = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         TABLE_COMMENT = table.Column<string>(type: "longtext", maxLength: 65535, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4")
            //     },
            //     constraints: table =>
            //     {
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "Capstone_Information_Schema_Tables");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "SurveyQuestions");

            // migrationBuilder.RenameTable(
            //     name: "Capstone_Information_Schema_Columns",
            //     newName: "Capstone_Information_Schema");
        }
    }
}
