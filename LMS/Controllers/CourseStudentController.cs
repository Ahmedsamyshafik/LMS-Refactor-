using LMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    public class CourseStudentController : Controller
    {
        #region Fields
        private readonly ICourseStudentService _courseStudentService;


        #endregion

        #region Ctor
        public CourseStudentController(ICourseStudentService courseStudentService)
        {
            _courseStudentService = courseStudentService;
        }
        #endregion

        #region Actions
        public async Task<IActionResult> GetAll()
        {
            var res = await _courseStudentService.GetAll();
            return View(res);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var x = await _courseStudentService.Delete(Id);
            return RedirectToAction("GetAll");
        }
        #endregion

    }
}
