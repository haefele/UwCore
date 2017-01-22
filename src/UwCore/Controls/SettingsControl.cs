using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UwCore.Controls
{
    [ContentProperty(Name = nameof(Groups))]
    public class SettingsControl : Control
    {
        public static readonly DependencyProperty GroupsProperty = DependencyProperty.Register(
            nameof(Groups), typeof(IList<SettingsGroup>), typeof(SettingsControl), new PropertyMetadata(default(IList<SettingsGroup>)));

        public IList<SettingsGroup> Groups
        {
            get { return (IList<SettingsGroup>) GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        public SettingsControl()
        {
            this.DefaultStyleKey = typeof(SettingsControl);

            this.Groups = new List<SettingsGroup>();
        }
    }
}