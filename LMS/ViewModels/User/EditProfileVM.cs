using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.User
{
    public class EditProfileVM
    {
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Phone { get; set; }


    }
}
