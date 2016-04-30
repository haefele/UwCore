using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwCore.Controls
{
    public class LoadingOverlay : Control
    {
        #region Properties
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            nameof(Message), typeof(string), typeof(LoadingOverlay), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            nameof(IsActive), typeof(bool), typeof(LoadingOverlay), new PropertyMetadata(default(bool)));

        public bool IsActive
        {
            get { return (bool)this.GetValue(IsActiveProperty); }
            set { this.SetValue(IsActiveProperty, value); }
        }
        #endregion

        #region Constructors
        public LoadingOverlay()
        {
            this.DefaultStyleKey = typeof(LoadingOverlay);
        }
        #endregion
    }
}