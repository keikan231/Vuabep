using CRS.Resources;
using DataAnnotationsExtensions;

namespace CRS.Common.DataAnnotations
{
    /// <summary>
    /// Validates maximum value of a number. Provides default localization resource settings.
    /// </summary>
    public class MaxExtendedAttribute : MaxAttribute
    {
        public MaxExtendedAttribute(int max)
            : base(max)
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "Max";
        }

        public MaxExtendedAttribute(double max)
            : base(max)
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "Max";
        }
    }
}