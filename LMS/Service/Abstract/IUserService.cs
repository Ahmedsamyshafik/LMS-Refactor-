using LMS.ViewModels.User;

namespace LMS.Service.Abstract
{
    public interface IUserService
    {
        Task EditProfile(EditProfileVM editProfileVM);
        Task<string> ChangePassword(ChangePasswordVM changePasswordVM);
        Task<EditProfileVM> GetUserData(string id);
    }
}
