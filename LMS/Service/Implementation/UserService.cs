using LMS.Models.Entities;
using LMS.Service.Abstract;
using LMS.ViewModels.User;
using Microsoft.AspNetCore.Identity;

namespace LMS.Service.Implementation
{
    public class UserService : IUserService
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;


        #endregion

        #region Ctor
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        #endregion

        #region Handle Functions

        public async Task<EditProfileVM> GetUserData(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            var res = new EditProfileVM()
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.PhoneNumber
            };
            return res;
        }
        public async Task EditProfile(EditProfileVM editProfileVM)
        {
            //normal update
            var user = await _userManager.FindByIdAsync(editProfileVM.Id);
            if (user != null)
            {
                user.Name = editProfileVM.Name;

                user.PhoneNumber = editProfileVM.Phone;

                await _userManager.UpdateAsync(user);
            }


        }

        public async Task<string> ChangePassword(ChangePasswordVM changePasswordVM)
        {
            //get user
            var user = await _userManager.FindByIdAsync(changePasswordVM.Id);
            if (user == null)
            {
                return "No User With this id !!";
            }
            //check pass?
            var res = await _userManager.ChangePasswordAsync(user, changePasswordVM.OldPass, changePasswordVM.NewPass);
            if (!res.Succeeded)
            {
                string err = string.Empty;
                foreach (var item in res.Errors)
                {
                    err += item.Description + " . ";
                }
                return err;
            }
            return "Success";
        }
        #endregion
    }
}
