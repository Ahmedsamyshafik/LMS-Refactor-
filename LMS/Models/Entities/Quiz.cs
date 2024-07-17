using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? FilePath { get; set; }

        [ForeignKey(nameof(courseId))]
        public int courseId { get; set; }
        public Course Course { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
    }
}
