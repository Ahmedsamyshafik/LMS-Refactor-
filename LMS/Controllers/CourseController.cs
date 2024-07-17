using LMS.Service.Abstract;
using LMS.ViewModels.Course;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    public class CourseController : Controller
    {
        #region Fields
        private readonly ICourseService _courseService;
        private readonly ICourseStudentService _courseStudentService;
        private readonly IVideoService _videoService;
        #endregion

        #region Ctor
        public CourseController(ICourseService courseService, ICourseStudentService courseStudentService, IVideoService videoService)
        {
            _courseService = courseService;
            _courseStudentService = courseStudentService;
            _videoService = videoService;
        }
        #endregion

        #region Actions
        [HttpGet]
        public IActionResult Index(int id)//page that contain videos
        {
            var videos = _videoService.getCourseVideos(id);
            ViewBag.CourseId = id;
            return View(videos);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var x = _courseService.GetAll();
            var res = new List<GetAllCoursesForAdminVM>();
            foreach (var item in x)
            {
                int number = _courseStudentService.GetStudentsCountInCourseById(item.Id);
                var temp = new GetAllCoursesForAdminVM()
                {
                    Id = item.Id,
                    imgPath = item.imgPath,
                    Name = item.Name,
                    Price = item.Price,
                    studentsIn = number
                };
                res.Add(temp);
            }
            return View(res);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddCourseVM courseVM)
        {
            await _courseService.AddCourse(courseVM);
            //tempData To save successfully
            return View();
        }
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var course = _courseService.GetById(id);
            string userId = GetUserId();
            if (userId != string.Empty)
            {
                course.CanEnroll = _courseStudentService.StudentInCourse(id, GetUserId());
            }
            else
            {
                course.CanEnroll = true;
            }

            if (course == null)
            {
                return RedirectToAction("index", "Home");
            }
            return View(course);
        }

        [HttpPost]
        public IActionResult Accept(int id)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.Delete(id);
            return RedirectToAction("GetAll");
        }

        #endregion

        #region Private
        private string GetUserId()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var uId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (uId == null) return string.Empty;
            return uId.Value;
        }
        #endregion
    }
}
