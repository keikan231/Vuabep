using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;

namespace CRS.Web.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [RequiredExtended]
        [Display(Name = "Mật khẩu")]
        [StringLengthExtended(30, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [RequiredExtended]
        [Display(Name = "Nhập lại mật khẩu")]
        [StringLengthExtended(30, MinimumLength = 6)]
        [EqualToExtended("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}