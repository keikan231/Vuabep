using System.Text;
using ActionMailer.Net.Mvc;
using CRS.Common;
using CRS.Web.ViewModels.Account;

namespace CRS.Web.Controllers
{
    public class MailController : MailerBase
    {
        //
        // GET: /Mail/

        public EmailResult ResetPasswordVerificationEmail(RequestResetPasswordViewModel vm)
        {
            To.Add(vm.Email);
            From = AppConfigs.SiteEmail;
            Subject = "Xác nhận quên mật khẩu - Vuabep.vn";
            MessageEncoding = Encoding.Unicode;
            return Email("ResetPasswordVerificationEmail", vm);
        }

        public EmailResult WelcomeEmail(RegisterViewModel vm)
        {
            To.Add(vm.Email);
            From = AppConfigs.SiteEmail;
            Subject = "Chào mừng tới Vuabep.vn";
            MessageEncoding = Encoding.Unicode;
            return Email("WelcomeEmail", vm);
        }

    }
}
