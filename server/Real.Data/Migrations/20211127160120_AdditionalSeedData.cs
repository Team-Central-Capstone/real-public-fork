using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class AdditionalSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SurveyQuestions",
                columns: new[] { "Id", "QuestionText", "QuestionType" },
                values: new object[] { 2, "What is your quest?", 2 });

            migrationBuilder.InsertData(
                table: "SurveyAnswers",
                columns: new[] { "Id", "AnswerText", "SurveyQuestionId" },
                values: new object[] { 4, "I want tacos", 2 });

            migrationBuilder.InsertData(
                table: "SurveyAnswers",
                columns: new[] { "Id", "AnswerText", "SurveyQuestionId" },
                values: new object[] { 5, "I seek the grail", 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SurveyAnswers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SurveyAnswers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SurveyQuestions",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
