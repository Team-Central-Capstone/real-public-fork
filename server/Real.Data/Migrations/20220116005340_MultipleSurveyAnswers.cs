using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class MultipleSurveyAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_SurveyAnswers_UserSurveyResponses_UserSurveyResponseId",
            //     table: "SurveyAnswers");

            // migrationBuilder.DropIndex(
            //     name: "IX_SurveyAnswers_UserSurveyResponseId",
            //     table: "SurveyAnswers");

            // migrationBuilder.DropColumn(
            //     name: "UserSurveyResponseId",
            //     table: "SurveyAnswers");

            migrationBuilder.CreateTable(
                name: "SurveyAnswerUserSurveyResponse",
                columns: table => new
                {
                    SurveyAnswersId = table.Column<int>(type: "int", nullable: false),
                    UserSurveyResponsesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyAnswerUserSurveyResponse", x => new { x.SurveyAnswersId, x.UserSurveyResponsesId });
                    table.ForeignKey(
                        name: "FK_SurveyAnswerUserSurveyResponse_SurveyAnswers_SurveyAnswersId",
                        column: x => x.SurveyAnswersId,
                        principalTable: "SurveyAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurveyAnswerUserSurveyResponse_UserSurveyResponses_UserSurve~",
                        column: x => x.UserSurveyResponsesId,
                        principalTable: "UserSurveyResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswerUserSurveyResponse_UserSurveyResponsesId",
                table: "SurveyAnswerUserSurveyResponse",
                column: "UserSurveyResponsesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyAnswerUserSurveyResponse");

            migrationBuilder.AddColumn<int>(
                name: "UserSurveyResponseId",
                table: "SurveyAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAnswers_UserSurveyResponseId",
                table: "SurveyAnswers",
                column: "UserSurveyResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAnswers_UserSurveyResponses_UserSurveyResponseId",
                table: "SurveyAnswers",
                column: "UserSurveyResponseId",
                principalTable: "UserSurveyResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
