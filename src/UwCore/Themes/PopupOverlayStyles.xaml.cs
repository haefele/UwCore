using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Caliburn.Micro;
using UwCore.Services.Navigation;
using INavigationService = UwCore.Services.Navigation.INavigationService;

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
            var popupNavigationService = IoC.Get<INavigationService>().Popup as PopupNavigationService;
            popupNavigationService?.Close();
        }

        private void ContentPresenter_OnLoaded(object sender, RoutedEventArgs e)
        {
            var contentPresenter = (ContentPresenter) sender;
            contentPresenter.RegisterPropertyChangedCallback(ContentPresenter.ContentProperty, this.ContentPresenter_OnContentChanged);

            this.ContentPresenter_OnContentChanged(contentPresenter, ContentPresenter.ContentProperty);
        }

        private void ContentPresenter_OnContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            var contentPresenter = (ContentPresenter) sender;
            var scrollViewer = (ScrollViewer)contentPresenter.Parent;
            var content = contentPresenter.Content as FrameworkElement;

            scrollViewer.HorizontalAlignment = double.IsNaN(content?.Width ?? double.NaN) 
                ? HorizontalAlignment.Stretch 
                : HorizontalAlignment.Center;

            scrollViewer.VerticalAlignment = double.IsNaN(content?.Height ?? double.NaN)
                ? VerticalAlignment.Stretch 
                : VerticalAlignment.Center;
        }
    }
}
