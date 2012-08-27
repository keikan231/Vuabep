using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CRS.Common.DataAnnotations;

namespace CRS.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Display(Name = "Tên đăng nhập")]
        [RequiredExtended]
        [StringLengthExtended(30)]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        [RequiredExtended]
        [StringLengthExtended(30)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Tự động đăng nhập lần sau")]
        public bool RememberMe { get; set; }
    }
}