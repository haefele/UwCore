using System;
using Windows.UI.Xaml.Data;

namespace UwCore.Converter
{
    public class BooleanToNullableBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool == false)
                return false;

            bool actualValue = (bool)value;
            return (bool?)actualValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}