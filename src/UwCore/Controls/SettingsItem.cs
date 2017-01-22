using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwCore.Controls
{
    public class SettingsItem : ContentControl
    {
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description), typeof(string), typeof(SettingsItem), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        public SettingsItem()
        {
            this.DefaultStyleKey = typeof(SettingsItem);
        }
    }
}