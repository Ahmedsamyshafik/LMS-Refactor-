using LMS.ViewModels.Course;

namespace LMS.Service.Abstract
{
    public interface ICourseService
    {
        Task AddCourse(AddCourseVM courseVM);
        List<GetCourseVM> GetAll();
        GetCourseVM GetById(int id);

        Task<string> Delete(int id);

    }
}
