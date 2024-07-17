namespace LMS.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImgPath { get; set; }
        public int Price { get; set; }

        public string? Description { get; set; }

        public bool free { get; set; }

        public ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
        public ICollection<CourseRequest> CourseRequests { get; set; } = new List<CourseRequest>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
        public ICollection<Material> Materials { get; set; } = new List<Material>(); // Add missing relationship
        public ICollection<Video> Videos { get; set; } = new List<Video>(); // Add missing relationship
    }
}
