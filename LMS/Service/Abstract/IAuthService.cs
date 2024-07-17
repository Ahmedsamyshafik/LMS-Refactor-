using LMS.Models.Entities;
using LMS.ViewModels.Account;

namespace LMS.Service.Abstract
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterVM registerVM);

        Task<string> LoginAsync(LoginVM loginVM);
        Task LogOut();
        Task<string> GetUserNameById(string userId);
        Task<ApplicationUser> GetUserById(string userId);
    }
}
