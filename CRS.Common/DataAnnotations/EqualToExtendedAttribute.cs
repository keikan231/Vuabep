using CRS.Resources;
using DataAnnotationsExtensions;

namespace CRS.Common.DataAnnotations
{
    /// <summary>
    /// Validates a property for equality against another property. Provides default localization resource settings.
    /// </summary>
    public class EqualToExtendedAttribute : EqualToAttribute
    {
        public EqualToExtendedAttribute(string otherProperty)
            : base(otherProperty)
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "MustMatch";
        }
    }
}