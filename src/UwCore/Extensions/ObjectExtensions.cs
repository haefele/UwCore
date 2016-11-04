using System.Collections.Generic;
using UwCore.Helpers;

namespace UwCore.Extensions
{
    public static class ObjectExtensions
    {
        public static void InjectValues(this object self, IDictionary<string, object> values)
        {
            if (values == null)
                return;

            foreach (var pair in values)
            {
                var property = self.GetType().GetPropertyCaseInsensitive(pair.Key);
                property?.SetValue(self, ConvertHelper.ConvertValue(property.PropertyType, pair.Value));
            }
        }

        public static bool AreValuesInjected(this object self, IDictionary<string, object> values)
        {
            foreach (KeyValuePair<string, object> param in values)
            {
                var property = self.GetType().GetPropertyCaseInsensitive(param.Key);

                if (property == null)
                    continue;

                var expectedValue = ConvertHelper.ConvertValue(property.PropertyType, param.Value);
                var actualValue = property.GetValue(self);

                if (object.Equals(expectedValue, actualValue) == false)
                    return false;
            }

            return true;
        }
    }
}