using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using UwCoreTest.Views.Test;
using INavigationService = UwCore.Services.Navigation.INavigationService;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UwCoreTest.Views.HeaderDetails
{
    public sealed partial class HeaderDetailsView : UserControl
    {
        public HeaderDetailsView()
        {
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            IoC.Get<INavigationService>().Popup.For<TestViewModel>().Navigate();
        }
    }
}
