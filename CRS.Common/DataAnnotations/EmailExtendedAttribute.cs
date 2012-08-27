using CRS.Resources;
using DataAnnotationsExtensions;

namespace CRS.Common.DataAnnotations
{
    /// <summary>
    /// Validates email address. Provides default localization resource settings.
    /// </summary>
    public class EmailExtendedAttribute : EmailAttribute
    {
        /*private const string EmailPattern =
           @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
*/
        public EmailExtendedAttribute()
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "Email";
        }
    }
}