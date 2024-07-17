using LMS.Models.Entities;
using LMS.Service.Abstract;
using LMS.ViewModels.Account;
using Microsoft.AspNetCore.Identity;

namespace LMS.Service.Implementation
{
    public class AuthService : IAuthService
    {
        #region Fields 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region Ctor
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Handle Function

        public async Task<string> RegisterAsync(RegisterVM registerVM)
        {
            //AppUser
            var user = new ApplicationUser()
            {
                Email = registerVM.Email,
                UserName = registerVM.Email,
                Name = registerVM.Name,
                PhoneNumber = registerVM.Phone
            };
            //Create
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                //return errors
                string err = string.Empty;
                foreach (var item in result.Errors)
                {
                    err += item.Description + " . ";
                }
                return err;
            }
            return "Success";
        }

        public async Task<string> LoginAsync(LoginVM loginVM)
        {

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user == null)
            {
                return "invalid email or password";
            }
            var res = await _userManager.CheckPasswordAsync(user, loginVM.Password);
            if (!res)
            {
                return "invalid email or password";

            }
            await _signInManager.SignInAsync(user, loginVM.RememberMe);

            return "Success";
        }

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GetUserNameById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user.Name;
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        #endregion



    }
}
