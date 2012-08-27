using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;

namespace CRS.Web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [RequiredExtended]
        [Display(Name = "Tên đăng nhập")]
        [StringLengthExtended(30, MinimumLength = 6)]
        public string Username { get; set; }

        [RequiredExtended]
        [Display(Name = "Mật khẩu")]
        [StringLengthExtended(30, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [RequiredExtended]
        [Display(Name = "Nhập lại mật khẩu")]
        [EqualToExtended("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [EmailExtended]
        [RequiredExtended]
        [Display(Name = "Email")]
        [StringLengthExtended(254)]
        public string Email { get; set; }
    }
}