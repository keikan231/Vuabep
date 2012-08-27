using CRS.Resources;
using DataAnnotationsExtensions;

namespace CRS.Common.DataAnnotations
{
    /// <summary>
    /// Validates minimum value of a number. Provides default localization resource settings.
    /// </summary>
    public class MinExtendedAttribute : MinAttribute
    {
        public MinExtendedAttribute(int min)
            : base(min)
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "Min";
        }

        public MinExtendedAttribute(double min)
            : base(min)
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "Min";
        }
    }
}