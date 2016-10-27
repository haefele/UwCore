using System;
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
            get { return (bool)this.GetValue(IsOpenProperty); }
            private set { this.SetValue(IsOpenProperty, value); }
        }

        public event EventHandler<PopupOverlayClosingEventArgs> Closing;
        public event EventHandler Closed;
        public event EventHandler Shown;

        public PopupOverlay()
        {
            this.DefaultStyleKey = typeof(PopupOverlay);
        }

        public bool Close()
        {
            if (this.IsOpen == false)
                return false;

            var closingEventArgs = new PopupOverlayClosingEventArgs();
            this.Closing?.Invoke(this, closingEventArgs);

            if (closingEventArgs.Cancel)
                return false;

            this.IsOpen = false;
            this.Closed?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public bool Show()
        {
            if (this.IsOpen)
                return false;

            this.IsOpen = true;
            this.Shown?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}