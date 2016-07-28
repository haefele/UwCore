using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwCore.Controls
{
    public class PopupOverlay : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), typeof(bool), typeof(PopupOverlay), new PropertyMetadata(default(bool)));

        public bool IsOpen
        {
            get { return (bool) GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public PopupOverlay()
        {
            this.DefaultStyleKey = typeof(PopupOverlay);
        }
    }
}