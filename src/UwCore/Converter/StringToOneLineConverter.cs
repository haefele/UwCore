using System;
using Windows.UI.Xaml.Data;
using UwCore.Extensions;

namespace UwCore.Converter
{
    public class StringToOneLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string s = value as string;

            if (s == null)
                return value;

            return s.MakeOneLiner();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}