using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class QuizExamTiming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAtUtc",
                table: "StudentQuizzes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentQuizzes",
                type: "int",
                nullable: false,
                // Existing rows are finished attempts; leaving them as InProgress would let students submit again over them.
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAtUtc",
                table: "StudentQuizzes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Quizzes",
                type: "int",
                nullable: false,
                // A zero-minute exam would expire the moment it is opened, so already stored quizzes get the platform default.
                defaultValue: 60);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartedAtUtc",
                table: "StudentQuizzes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentQuizzes");

            migrationBuilder.DropColumn(
                name: "SubmittedAtUtc",
                table: "StudentQuizzes");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Quizzes");
        }
    }
}
