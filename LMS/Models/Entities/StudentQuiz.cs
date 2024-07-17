using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class StudentQuiz
    {
        public int Id { get; set; }
        public int MaxDegree { get; set; }
        public int ExamDegree { get; set; }
        public string? StudentAnswerPath { get; set; }

        [ForeignKey(nameof(Student))]
        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
