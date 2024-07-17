using LMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    public class CourseRequestController : Controller
    {
        #region Fields
        private readonly ICourseRequestService _courseRequestService;

        #endregion

        #region Ctor
        public CourseRequestController(ICourseRequestService courseRequestService)
        {
            _courseRequestService = courseRequestService;
        }
        #endregion

        #region Actions
        //  [HttpPost]
        public async Task<IActionResult> Enroll(int id)//course id
        {
            var userId = GetUserId();
            if (userId == string.Empty)
            {
                return RedirectToAction("Login", "Account");
            }
            //service to check if in Course request => nothing , Add to table--- Free Course=> Assign
            string response = await _courseRequestService.Add(id, userId);

            return RedirectToAction("index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var CR = await _courseRequestService.GetAll();
            return View(CR);
        }
        public async Task<IActionResult> Delete(int id)
        {
            string res = await _courseRequestService.Delete(id);
            //check res
            return RedirectToAction("GetAll");
        }
        public async Task<IActionResult> Accept(int id)
        {
            string res = await _courseRequestService.Accept(id);
            //check res
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
