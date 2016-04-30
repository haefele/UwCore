using System;
using Windows.UI.Xaml.Data;

namespace UwCore.Converter
{
    public class FormatStringConverter : IValueConverter
    {
        public string PlaceholderString { get; set; }

        public string FormatString { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = this.GetDisplayFormat();

            var result = string.IsNullOrWhiteSpace(format) 
                ? value?.ToString()
                : string.Format(format, value);

            if (string.IsNullOrWhiteSpace(result))
                return this.PlaceholderString;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private string GetDisplayFormat()
        {
            if (string.IsNullOrWhiteSpace(this.FormatString))
                return string.Empty;

            if (this.FormatString.Contains("{"))
                return this.FormatString;

            return string.Format("{{0:{0}}}", this.FormatString);
        }
    }
}