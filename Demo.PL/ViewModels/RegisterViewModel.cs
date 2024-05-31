using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage ="First Name Is Required")]
		public string FName { get; set; }
		[Required(ErrorMessage ="Last Name Is Required")]
		public string LName { get; set; }
		[EmailAddress(ErrorMessage ="Invalid Email")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Password Name Is Required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Required(ErrorMessage = "Confirm Password Name Is Required")]
		[DataType(DataType.Password)]
		[Compare("Password",ErrorMessage ="Password Dos not Match")]
		public string ConfirmPassword { get; set; }
		public bool IAgree { get; set; }




	}
}
