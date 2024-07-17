using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int CorrectAnswer { get; set; }

        public ICollection<Option> Options { get; set; } = new List<Option>();

        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
