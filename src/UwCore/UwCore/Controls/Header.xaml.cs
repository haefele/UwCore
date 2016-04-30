using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwCore.Controls
{
    public sealed partial class Header : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(Header), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty NavigationButtonVisibilityProperty = DependencyProperty.Register(
            "NavigationButtonVisibility", typeof (Visibility), typeof (Header), new PropertyMetadata(default(Visibility)));

        public Visibility NavigationButtonVisibility
        {
            get { return (Visibility)this.GetValue(NavigationButtonVisibilityProperty); }
            set { this.SetValue(NavigationButtonVisibilityProperty, value); }
        }

        public event EventHandler<RoutedEventArgs> NavigationButtonClick;

        public Header()
        {
            this.InitializeComponent();
        }

        private void NavigationButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationButtonClick?.Invoke(this, e);
        }
    }
}