using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Video
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VideoPath { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
