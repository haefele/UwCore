using System;
using System.Collections.Generic;
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
    }
}
