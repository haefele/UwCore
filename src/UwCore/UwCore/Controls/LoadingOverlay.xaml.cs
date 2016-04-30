using Windows.UI.Xaml;

namespace UwCore.Controls
{
    public partial class LoadingOverlay
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(LoadingOverlay), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(LoadingOverlay), new PropertyMetadata(default(bool)));

        public bool IsActive
        {
            get { return (bool)this.GetValue(IsActiveProperty); }
            set { this.SetValue(IsActiveProperty, value); }
        }

        public LoadingOverlay()
        {
            this.InitializeComponent();
        }
    }
}
