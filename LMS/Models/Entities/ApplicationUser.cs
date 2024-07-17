using Microsoft.AspNetCore.Identity;

namespace LMS.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
        public ICollection<CourseRequest> CourseRequests { get; set; } = new List<CourseRequest>();
        public ICollection<StudentQuiz> StudentQuizzes { get; set; } = new List<StudentQuiz>();
    }
}
