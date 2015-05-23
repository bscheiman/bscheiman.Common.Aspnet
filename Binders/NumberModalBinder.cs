#region
using System;
using System.Globalization;
using System.Web.Mvc;

#endregion

namespace bscheiman.Common.Aspnet.Binders {
    // Kudos to http://stackoverflow.com/questions/793459/how-to-set-decimal-separators-in-asp-net-mvc-controllers/5117441#5117441
    // and http://www.crydust.be/blog/2009/07/30/custom-model-binder-to-avoid-decimal-separator-problems/
    public class NumberModalBinder : DefaultModelBinder {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            object result = null;

            var modelName = bindingContext.ModelName;
            var attemptedValue = bindingContext.ValueProvider.GetValue(modelName).AttemptedValue;

            var wantedSeperator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var alternateSeperator = (wantedSeperator == "," ? "." : ",");

            if (attemptedValue.IndexOf(wantedSeperator, StringComparison.Ordinal) == -1 &&
                attemptedValue.IndexOf(alternateSeperator, StringComparison.Ordinal) != -1)
                attemptedValue = attemptedValue.Replace(alternateSeperator, wantedSeperator);

            try {
                if (bindingContext.ModelMetadata.IsNullableValueType && string.IsNullOrWhiteSpace(attemptedValue))
                    return null;

                result = decimal.Parse(attemptedValue, NumberStyles.Any);
            } catch (FormatException e) {
                bindingContext.ModelState.AddModelError(modelName, e);
            }

            return result;
        }
    }
}