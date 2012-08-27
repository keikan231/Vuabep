using System.ComponentModel.DataAnnotations;
using CRS.Resources;

namespace CRS.Common.DataAnnotations
{
    /// <summary>
    /// Validates length of a string. Provides default localization resource settings.
    /// </summary>
    public class StringLengthExtendedAttribute : StringLengthAttribute
    {
        public StringLengthExtendedAttribute(int maximumLength)
            : base(maximumLength)
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
        }

        public override string FormatErrorMessage(string name)
        {
            ErrorMessageResourceName = MinimumLength == 0 ? "StringLengthMax" : "StringLengthMinMax";

            return string.Format(ErrorMessageString, name, MaximumLength, MinimumLength);
        }
    }
}