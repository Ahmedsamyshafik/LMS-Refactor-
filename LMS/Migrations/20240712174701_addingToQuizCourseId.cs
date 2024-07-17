using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class addingToQuizCourseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "courseId",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_courseId",
                table: "Quizzes",
                column: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_courseId",
                table: "Quizzes",
                column: "courseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_courseId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_courseId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "courseId",
                table: "Quizzes");
        }
    }
}
