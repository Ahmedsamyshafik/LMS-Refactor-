using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Entities
{
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PdfPath { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
