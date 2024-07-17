using LMS.Models.Entities;
using LMS.ViewModels.CourseRequest;

namespace LMS.Service.Abstract
{
    public interface ICourseStudentService
    {
        bool StudentInCourse(int courseId, string userid);
        Task Add(CourseStudent obj);
        Task<List<GetAllCoursesRequestsVM>> GetAll();
        Task<string> Delete(int id);
        int GetStudentsCountInCourseById(int id);
    }
}
