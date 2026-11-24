using LMS.Helper;
using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;
using LMS.ViewModels.CourseRequest;
using Microsoft.EntityFrameworkCore;

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
                    CourseId = item.CourseId,
                    CourseName = course?.Name ?? string.Empty,
                    CoursePrice = course == null ? string.Empty : PriceFormatter.Format(course.Price),
                    StudentName = await _authService.GetUserNameById(item.UserId),
                    StudentId = item.UserId,
                    Time = item.Time,

                };
                result.Add(temp);

            }
            return result;
        }

        /// <summary>
        /// Courses one student is enrolled in, resolved in the database. The navigation menus use it so a
        /// learner only gets links for their own courses instead of a list built from exam attempts.
        /// </summary>
        public List<GetAllCoursesRequestsVM> GetByStudentId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<GetAllCoursesRequestsVM>();
            }

            return _courseStudentRepo.GetTableNoTracking()
                .Include(x => x.Course)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Id)
                .Select(x => new GetAllCoursesRequestsVM
                {
                    Id = x.Id,
                    CourseId = x.CourseId,
                    CourseName = x.Course.Name,
                    CoursePrice = PriceFormatter.Format(x.Course.Price),
                    StudentId = x.UserId,
                    Time = x.Time
                })
                .ToList();
        }

        public async Task<CourseStudent> GetById(int id)
        {
            return await _courseStudentRepo.GetByIdAsync(id);
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
