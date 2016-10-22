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
            get { return (bool) GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public event EventHandler<PopupOverlayClosingEventArgs> Closing;
        public event EventHandler Closed;

        public PopupOverlay()
        {
            this.DefaultStyleKey = typeof(PopupOverlay);
        }

        public void Close()
        {
            var closingEventArgs = new PopupOverlayClosingEventArgs();
            this.Closing?.Invoke(this, closingEventArgs);

            if (closingEventArgs.Cancel == false)
            {
                this.IsOpen = false;
                this.Closed?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public class PopupOverlayClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}