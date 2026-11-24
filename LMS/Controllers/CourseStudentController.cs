using LMS.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class CourseStudentController : Controller
    {
        #region Fields
        private readonly ICourseStudentService _courseStudentService;
        private readonly IStudentQuizService _studentQuizService;


        #endregion

        #region Ctor
        public CourseStudentController(ICourseStudentService courseStudentService, IStudentQuizService studentQuizService)
        {
            _courseStudentService = courseStudentService;
            _studentQuizService = studentQuizService;
        }
        #endregion

        #region Actions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var res = await _courseStudentService.GetAll();
            return View(res);
        }
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {

            var x = await _courseStudentService.Delete(Id);

            return RedirectToAction("GetAll");
        }
        #endregion

    }
}
