using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LMS.ViewModels.Account
{
	public class RegisterVM
	{
		[Required]
		public string Email { get; set; }
		[Required]

		public string Name { get; set; }
		[Required]
		public string Phone { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DisplayName("Confirm Password")]
		[Required]
		[DataType(DataType.Password)]
		public string CoPassword { get; set; }
	}
}
