using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "NewPassword Name Is Required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "ConfirmNewPassword Name Is Required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="Password Dos not Match")]
        public string ConfirmNewPassword { get; set; }
    }
}
