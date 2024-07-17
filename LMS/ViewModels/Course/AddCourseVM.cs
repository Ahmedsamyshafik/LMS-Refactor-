namespace LMS.ViewModels.Course
{
    public class AddCourseVM
    {
        public string Name { get; set; }

        public int Price { get; set; }
        public string? Description { get; set; }

        public IFormFile file { get; set; }
    }
}
