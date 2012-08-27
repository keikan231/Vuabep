using System.Web.Security;

namespace CRS.Common.Cryptography
{
    public class PasswordCryptography
    {
        public static string HashPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, Constants.EncryptionType);
        }
    }
}