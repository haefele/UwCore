using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Microsoft.Toolkit.Uwp.UI;
using UwCore.Controls;

namespace UwCore.Themes
{
    public partial class PopupOverlayStyles : ResourceDictionary
    {
        public PopupOverlayStyles()
        {
            this.InitializeComponent();
        }

        private void PopupOverlayBackground_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var popupOverlay = ((FrameworkElement)sender).FindVisualAscendant<PopupOverlay>();
            popupOverlay.Close();
        }

        private void ContentPresenter_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.AttachToEscapeKeyToClosePopup(sender);
            this.AttachToContentChangedEventToAdjustMaxWidthAndMaxHeight(sender);
        }

        private void AttachToContentChangedEventToAdjustMaxWidthAndMaxHeight(object sender)
        {
            var contentPresenter = (ContentPresenter) sender;
            contentPresenter.RegisterPropertyChangedCallback(ContentPresenter.ContentProperty, this.ContentPresenter_OnContentChanged);

            this.ContentPresenter_OnContentChanged(contentPresenter, ContentPresenter.ContentProperty);
        }

        private void AttachToEscapeKeyToClosePopup(object sender)
        {
            var popupOverlay = ((FrameworkElement) sender).FindVisualAscendant<PopupOverlay>();
            popupOverlay.Dispatcher.AcceleratorKeyActivated += (s, e) =>
            {
                if (e.VirtualKey == VirtualKey.Escape && popupOverlay.IsOpen)
                {
                    popupOverlay.Close();
                }
            };
        }

        private void ContentPresenter_OnContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            var contentPresenter = (ContentPresenter) sender;
            var grid = (Grid)contentPresenter.Parent;
            var content = contentPresenter.Content as FrameworkElement;

            grid.MaxWidth = this.GetMax(content, f => f.Width, f => f.MaxWidth) 
                + grid.BorderThickness.Left 
                + grid.BorderThickness.Right;

            grid.MaxHeight = this.GetMax(content, f => f.Height, f => f.MaxHeight) 
                + grid.BorderThickness.Bottom 
                + grid.BorderThickness.Top;
        }

        private double GetMax(FrameworkElement element, Func<FrameworkElement, double> selector, Func<FrameworkElement, double> maxSelector)
        {
            if (element == null)
                return double.PositiveInfinity;

            if (double.IsInfinity(maxSelector(element)) == false && double.IsNaN(maxSelector(element)) == false)
                return maxSelector(element);

            if (double.IsInfinity(selector(element)) == false && double.IsNaN(selector(element)) == false)
                return selector(element);

            return double.PositiveInfinity;
        }
    }
}
