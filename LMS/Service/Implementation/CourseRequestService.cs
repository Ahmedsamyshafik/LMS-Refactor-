using LMS.Models.Entities;
using LMS.Repository.EntityRepos.IRepo;
using LMS.Service.Abstract;
using LMS.ViewModels.CourseRequest;

namespace LMS.Service.Implementation
{
    public class CourseRequestService : ICourseRequestService
    {
        #region Fields
        private readonly ICourseRequestRepo _courseRequestRepo;
        private readonly IAuthService _authService;
        private readonly ICourseService _courseService;
        private readonly ICourseStudentService _courseStudentService;
        #endregion
        #region Ctor
        public CourseRequestService(ICourseRequestRepo courseRequestRepo, ICourseService courseService, IAuthService authService, ICourseStudentService courseStudentService)
        {
            _courseRequestRepo = courseRequestRepo;
            _courseService = courseService;
            _authService = authService;
            _courseStudentService = courseStudentService;
        }
        #endregion
        #region Handle Functions
        public async Task<string> Add(int courseId, string userId)
        {
            var rec = _courseRequestRepo.GetTableNoTracking().Where(x => x.UserId == userId && x.CourseId == courseId).FirstOrDefault();
            if (rec == null)
            {
                //---add
                //check course price?
                var C_Price = _courseService.GetById(courseId).Price;
                if (C_Price != 0)
                {
                    var CourseRequestObject = new CourseRequest()
                    {
                        CourseId = courseId,
                        UserId = userId,
                        Time = DateTime.Now
                    };
                    await _courseRequestRepo.AddAsync(CourseRequestObject);
                }
                else
                {
                    var CourseStudent = new CourseStudent()
                    {
                        CourseId = courseId,
                        Time = DateTime.Now,
                        UserId = userId,
                    };
                    await _courseStudentService.Add(CourseStudent);
                }


                return "Success";
            }
            else
            {
                //Already Assigned !
                return "Already Assigned";
            }
        }

        public async Task<List<GetAllCoursesRequestsVM>> GetAll()
        {
            var allCoursesRequests = _courseRequestRepo.GetTableNoTracking().ToList();
            List<GetAllCoursesRequestsVM> result = new();
            //mapping
            foreach (var item in allCoursesRequests)
            {
                var course = _courseService.GetById(item.CourseId);
                var temp = new GetAllCoursesRequestsVM()
                {
                    CourseName = course.Name,
                    StudentName = await _authService.GetUserNameById(item.UserId),
                    CoursePrice = course.Price.ToString(),
                    Id = item.Id,
                    Time = item.Time
                };
                result.Add(temp);
            }
            return result;
        }

        public async Task<string> Delete(int id)
        {
            var CR = _courseRequestRepo.GetTableNoTracking().Where(x => x.Id == id).FirstOrDefault();
            if (CR == null)
            {
                return "Error";
            }

            await _courseRequestRepo.DeleteAsync(CR);
            return "Success";
        }
        public async Task<string> Accept(int id)
        {
            var CR = _courseRequestRepo.GetTableNoTracking().Where(x => x.Id == id).FirstOrDefault();
            if (CR == null)
            {
                return "Error";
            }
            //Add To Course -yang close,
            var CourseStudent = new CourseStudent()
            {
                CourseId = CR.CourseId,
                Time = DateTime.Now,
                UserId = CR.UserId
            };
            await _courseStudentService.Add(CourseStudent);
            await _courseRequestRepo.DeleteAsync(CR);
            return "Success";
        }

        #endregion

    }
}
