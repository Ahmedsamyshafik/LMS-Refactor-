using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;
using LMS.ViewModels.CourseRequest;

namespace LMS.Service.Implementation
{
    public class CourseStudentService : ICourseStudentService
    {
        #region Fields
        private readonly ICourseStudentRepo _courseStudentRepo;
        private readonly ICourseService _courseService;
        private readonly IAuthService _authService;
        #endregion

        #region Ctor
        public CourseStudentService(ICourseStudentRepo courseStudentRepo, ICourseService courseService, IAuthService authService)
        {
            _courseStudentRepo = courseStudentRepo;
            _courseService = courseService;
            _authService = authService;
        }
        #endregion

        #region Handle Functions

        public bool StudentInCourse(int courseId, string userid)
        {
            var rec = _courseStudentRepo.GetTableNoTracking().Where(x => x.UserId == userid && x.CourseId == courseId);
            if (rec.Any()) return true;
            return false;
        }

        public async Task Add(CourseStudent obj)
        {
            await _courseStudentRepo.AddAsync(obj);
        }

        public async Task<List<GetAllCoursesRequestsVM>> GetAll()
        {
            var result = new List<GetAllCoursesRequestsVM>();
            var lst = _courseStudentRepo.GetTableAsList();
            foreach (var item in lst)
            {
                var course = _courseService.GetById(item.CourseId);
                var temp = new GetAllCoursesRequestsVM()
                {
                    Id = item.Id,
                    CourseName = course.Name,
                    CoursePrice = course.Price.ToString(),
                    StudentName = await _authService.GetUserNameById(item.UserId),
                    Time = item.Time,

                };
                result.Add(temp);

            }
            return result;
        }

        public int GetStudentsCountInCourseById(int id)
        {
            return _courseStudentRepo.GetTableNoTracking()
                                     .Where(x => x.CourseId == id)
                                     .Select(x => x.UserId)
                                     .Distinct()
                                     .Count();

        }

        public async Task<string> Delete(int id)
        {
            var rec = _courseStudentRepo.GetTableNoTracking().Where(x => x.Id == id).FirstOrDefault();
            if (rec != null)
            {
                await _courseStudentRepo.DeleteAsync(rec);
            }
            return "";
        }
        #endregion



    }
}
