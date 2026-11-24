namespace LMS.ViewModels.QuizVM
{
    public class AddWritten
    {
        public string Name { get; set; }
        public string description { get; set; }
        public int CourseId { get; set; }
        public int DurationMinutes { get; set; } = 60;
        public IFormFile file { get; set; }

    }
}
