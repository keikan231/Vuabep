using System.ComponentModel.DataAnnotations;
using CRS.Resources;

namespace CRS.Common.DataAnnotations
{
    /// <summary>
    /// Validates a property to be required. Provides default localization resource settings.
    /// </summary>
    public class RequiredExtendedAttribute : RequiredAttribute
    {
        public RequiredExtendedAttribute()
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "Required";
        }
    }
}