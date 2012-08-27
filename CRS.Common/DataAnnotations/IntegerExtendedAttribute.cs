using CRS.Resources;
using DataAnnotationsExtensions;

namespace CRS.Common.DataAnnotations
{
        /// <summary>
        /// Validates a field must be an integer. Provides default localization resource settings.
        /// </summary>
        public class IntegerExtendedAttribute : IntegerAttribute
        {
            public IntegerExtendedAttribute()
            {
                ErrorMessageResourceType = typeof(ValidationMessages);
                ErrorMessageResourceName = "Integer";
            }
        }
}