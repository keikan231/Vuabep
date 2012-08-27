using System.ComponentModel;
using System.Web.Mvc;

namespace CRS.Web.Framework.ModelBinders
{
    /// <summary>
    /// Model binder to trim all input strings and consider empty strings as null
    /// </summary>
    public class TrimModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext,
                                            ModelBindingContext bindingContext,
                                            PropertyDescriptor propertyDescriptor,
                                            object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var stringValue = (string)value;
                if (stringValue != null)
                    stringValue = stringValue.Trim();
                if (stringValue == string.Empty)
                    stringValue = null;

                value = stringValue;
            }

            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
        }
    }
}