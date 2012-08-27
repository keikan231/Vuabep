using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CRS.Common.DataAnnotations;

namespace CRS.Web.ViewModels.Account
{
    public class RequestResetPasswordViewModel
    {
        [RequiredExtended]
        [EmailExtended]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Username { get; set; }

        public string ResetPasswordKey { get; set; }

        public string ResetPasswordLink { get; set; }
    }
}