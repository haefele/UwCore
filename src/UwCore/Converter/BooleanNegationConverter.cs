using System;
using Windows.UI.Xaml.Data;

namespace UwCore.Converter
{
    public class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            bool? booleanValue = value as bool?;

            booleanValue = !booleanValue;

            if (targetType == typeof(bool))
                return booleanValue ?? true;

            return booleanValue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}