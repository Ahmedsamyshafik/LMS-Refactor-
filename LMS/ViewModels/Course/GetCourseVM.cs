namespace LMS.ViewModels.Course
{
    public class GetCourseVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Price { get; set; }

        public string? Description { get; set; }

        public bool CanEnroll { get; set; }
        public string imgPath { get; set; }
    }
}
