using LMS.Service.Abstract;
using LMS.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Controllers
{
    public class AccountController : Controller
    {
        #region Fields
        private readonly IAuthService _authService;

        #endregion

        #region ctor
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion

        #region Actions

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                string response = await _authService.RegisterAsync(registerVM);
                if (response == "Success")
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", response);
            }
            return View(registerVM);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                string response = await _authService.LoginAsync(loginVM);
                if (response == "Success")
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", response);

            }
            return View(loginVM);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _authService.LogOut();
            return RedirectToAction("Login");
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
