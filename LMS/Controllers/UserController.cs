using LMS.Service.Abstract;
using LMS.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    public class UserController : Controller
    {
        #region Fields
        private readonly IUserService _userService;

        #endregion

        #region Ctor
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region Actions


        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var id = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var vm = await _userService.GetUserData(id);
            if (vm == null) return NotFound();
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVM editProfileVM)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var id = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                editProfileVM.Id = id;
                await _userService.EditProfile(editProfileVM);
                //Success Message
            }

            return View(editProfileVM);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var id = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                changePasswordVM.Id = id;
                string response = await _userService.ChangePassword(changePasswordVM);
                if (response == "Success")
                {
                    //success message!
                    return RedirectToAction("EditProfile");
                }
                ModelState.AddModelError("", response);
            }
            return View(changePasswordVM);
        }


        #endregion


    }
}
