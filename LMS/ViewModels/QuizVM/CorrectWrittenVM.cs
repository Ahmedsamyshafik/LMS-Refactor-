using LMS.Models.Entities;

namespace LMS.ViewModels.QuizVM
{
    public class CorrectWrittenVM
    {
        public int CourseId { get; set; }

        public List<GradeableAttempt> Attempts { get; set; } = new();
    }

    public class GradeableAttempt
    {
        public int Id { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string QuizName { get; set; } = string.Empty;

        public string? StudentAnswerPath { get; set; }

        public int ExamDegree { get; set; }

        public int MaxDegree { get; set; }
    }
}
