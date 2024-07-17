using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.Account
{
    public class LoginVM
    {
        [Required]
        public string Email { get; set; }
        [Required]

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
