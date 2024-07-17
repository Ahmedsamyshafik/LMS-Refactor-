using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Option
    {
        public int Id { get; set; }
        public string Text { get; set; }

        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
