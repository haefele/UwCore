using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UwCore.Extensions;

namespace UwCore.Services.Navigation
{
    public static class ParametersHelper
    {
        private static PropertyInfo GetParametersProperty(object instance)
        {
            var parametersProperty =
                instance.GetType().GetPropertyCaseInsensitive("Parameter") ??
                instance.GetType().GetPropertyCaseInsensitive("Parameters") ??
                instance.GetType().GetPropertyCaseInsensitive("Param");

            if (parametersProperty == null)
                return null;

            if (parametersProperty.CanRead == false || parametersProperty.CanWrite == false)
                return null;

            return parametersProperty;
        }

        public static void InjectParameter(object self, IDictionary<string, object> values)
        {
            if (values == null || values.Any() == false)
                return;

            var parametersProperty = GetParametersProperty(self);
            if (parametersProperty != null)
            {
                var parameter = Activator.CreateInstance(parametersProperty.PropertyType);
                parameter.InjectValues(values);

                parametersProperty.SetValue(self, parameter);
            }
            else
            {
                self.InjectValues(values);
            }
        }

        public static bool AreParameterInjected(object self, IDictionary<string, object> values)
        {
            if (values == null || values.Any() == false)
                return true;

            var parametersProperty = GetParametersProperty(self);

            if (parametersProperty != null)
            {
                var parameter = parametersProperty.GetValue(self);
                return parameter != null && parameter.AreValuesInjected(values);
            }
            else
            {
                return self.AreValuesInjected(values);
            }
        }
    }
}