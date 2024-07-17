using LMS.ViewModels.CourseRequest;

namespace LMS.Service.Abstract
{
    public interface ICourseRequestService
    {
        Task<string> Add(int courseId, string userId);
        Task<List<GetAllCoursesRequestsVM>> GetAll();
        Task<string> Accept(int id);
        Task<string> Delete(int id);
    }
}
