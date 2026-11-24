using LMS.Service.Abstract;
using LMS.ViewModels.Course;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ICourseRequestService _courseRequestService;
        #endregion

        #region Ctor
        public CourseController(ICourseService courseService, ICourseStudentService courseStudentService, IVideoService videoService, ICourseRequestService courseRequestService)
        {
            _courseService = courseService;
            _courseStudentService = courseStudentService;
            _videoService = videoService;
            _courseRequestService = courseRequestService;
        }
        #endregion

        #region Actions
        [HttpGet]
        [Authorize]

        public IActionResult Index(int id)//page that contain videos
        {
            if (_courseService.GetById(id) == null)
            {
                return NotFound();
            }

            var videos = _videoService.getCourseVideos(id);
            ViewBag.CourseId = id;
            return View(videos);
        }
        [Authorize]
        public async Task<IActionResult> DisplayVideo(int id)//vid id 
        {
            var video = await _videoService.GetById(id);
            if (video == null)
            {
                return NotFound();
            }
            return View(video);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]

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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (course == null) return NotFound();
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                course.CanEnrollStuts = "none";
            }
            else
            {
                var EnrollmentStatus = _courseRequestService.checkEnrolmentStatus(userId, id);
                if (EnrollmentStatus)
                {//enroll
                    course.CanEnrollStuts = "Enroll";
                    //--check if he is in course already !
                    bool res = _courseStudentService.StudentInCourse(id, userId);
                    if (res)
                        course.CanEnrollStuts = "GoToCourse"; //check if he is in course already !


                }
                else
                {//pending
                    course.CanEnrollStuts = "Waiting";
                }
            }




            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Accept(int id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
