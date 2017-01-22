using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UwCore.Controls
{
    [ContentProperty(Name = nameof(Settings))]
    public class SettingsGroup : Control
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header), typeof(string), typeof(SettingsGroup), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description), typeof(string), typeof(SettingsGroup), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)this.GetValue(DescriptionProperty); }
            set { this.SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
            nameof(Settings), typeof(IList<SettingsItem>), typeof(SettingsGroup), new PropertyMetadata(default(IList<SettingsItem>)));

        public IList<SettingsItem> Settings
        {
            get { return (IList<SettingsItem>)this.GetValue(SettingsProperty); }
            set { this.SetValue(SettingsProperty, value); }
        }

        public SettingsGroup()
        {
            this.DefaultStyleKey = typeof(SettingsGroup);

            this.Settings = new List<SettingsItem>();
        }
    }
}