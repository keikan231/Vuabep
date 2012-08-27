using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CRS.Resources;

namespace CRS.Common.DataAnnotations
{
    public class RangeExtendedAttribute : RangeAttribute
    {
        public RangeExtendedAttribute(int min, int max)
            : base(min, max)
        {
            SetDefault();
        }

        public RangeExtendedAttribute(double min, double max)
            : base(min, max)
        {
            SetDefault();
        }

        private void SetDefault()
        {
            ErrorMessageResourceType = typeof(ValidationMessages);
            ErrorMessageResourceName = "Range";
        }
    }
}