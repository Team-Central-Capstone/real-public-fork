using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class information_schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentDisposition",
                table: "UserImages");


            migrationBuilder.Sql(@"
                create or replace view Capstone_Information_Schema_Columns
                as
                select *
                from information_schema.COLUMNS
                order by table_catalog, table_schema, table_name, ordinal_position");

            migrationBuilder.Sql(@"
                create or replace view Capstone_Information_Schema_Tables
                as
                SELECT *
                FROM INFORMATION_SCHEMA.TABLES");


            // migrationBuilder.CreateTable(
            //     name: "Capstone_Information_Schema",
            //     columns: table => new
            //     {
            //         TABLE_CATALOG = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         TABLE_SCHEMA = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         TABLE_NAME = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         COLUMN_NAME = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         ORDINAL_POSITION = table.Column<int>(type: "int", nullable: false),
            //         COLUMN_DEFAULT = table.Column<string>(type: "longtext", maxLength: 65535, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         IS_NULLABLE = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         DATA_TYPE = table.Column<string>(type: "longtext", nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         CHARACTER_MAXIMUM_LENGTH = table.Column<long>(type: "bigint", nullable: true),
            //         CHARACTER_OCTET_LENGTH = table.Column<long>(type: "bigint", nullable: true),
            //         NUMERIC_PRECISION = table.Column<long>(type: "bigint", nullable: true),
            //         NUMERIC_SCALE = table.Column<long>(type: "bigint", nullable: true),
            //         DATETIME_PRECISION = table.Column<int>(type: "int", nullable: true),
            //         CHARACTER_SET_NAME = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         COLLATION_NAME = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         COLUMN_TYPE = table.Column<string>(type: "longtext", maxLength: 16777215, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         COLUMN_KEY = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         EXTRA = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         PRIVILEGES = table.Column<string>(type: "varchar(154)", maxLength: 154, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         COLUMN_COMMENT = table.Column<string>(type: "longtext", maxLength: 65535, nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         GENERATION_EXPRESSION = table.Column<string>(type: "longtext", nullable: true)
            //             .Annotation("MySql:CharSet", "utf8mb4"),
            //         SRS_ID = table.Column<int>(type: "int", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //     })
            //     .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop view Capstone_Information_Schema_Columns");
            migrationBuilder.Sql("drop view Capstone_Information_Schema_Tables");
            // migrationBuilder.DropTable(
            //     name: "Capstone_Information_Schema");

            migrationBuilder.AddColumn<string>(
                name: "ContentDisposition",
                table: "UserImages",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
