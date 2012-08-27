using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;

namespace CRS.Web.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Mật khẩu cũ")]
        [RequiredExtended]
        [StringLengthExtended(30)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [RequiredExtended]
        [StringLengthExtended(30, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Nhập lại mật khẩu mới")]
        [RequiredExtended]
        [StringLengthExtended(30, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [EqualToExtended("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}