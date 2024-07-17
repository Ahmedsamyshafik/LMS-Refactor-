namespace LMS.ViewModels.QuizVM
{
    public class AddWritten
    {
        public string Name { get; set; }
        public string description { get; set; }
        public int CourseId { get; set; }
        public IFormFile file { get; set; }

    }
}
