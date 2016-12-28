using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using PopupOverlayClass = UwCore.Controls.PopupOverlay;

namespace UwCore.Hamburger
{
    public sealed partial class HamburgerView : Page
    {
        public HamburgerViewModel ViewModel => this.DataContext as HamburgerViewModel;

        public HamburgerView()
        {
            this.InitializeComponent();

            this.PopupOverlay.RegisterPropertyChangedCallback(PopupOverlayClass.IsOpenProperty, this.OnPopupOverlayIsOpenChanged);
            this.LoadingOverlay.RegisterPropertyChangedCallback(Controls.LoadingOverlay.IsActiveProperty, this.OnLoadingOverlayIsActiveChanged);
        }

        private void ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (HamburgerItem)e.ClickedItem;
            clickedItem.Execute();

            if (this.WindowSize.CurrentState == this.Narrow)
            {
                this.Navigation.IsPaneOpen = false;
            }
        }

        private void Header_OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {
            this.Navigation.IsPaneOpen = true;
        }

        private void NavigationPane_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (this.WindowSize.CurrentState != this.Narrow)
                return;

            if (this.Navigation.IsPaneOpen)
                return;

            if (e.Cumulative.Translation.X > 20)
            {
                this.Navigation.IsPaneOpen = true;
            }
        }
        
        private void OnLoadingOverlayIsActiveChanged(DependencyObject sender, DependencyProperty dp)
        {
            this.UpdateBackgroundBlur();
        }

        private void OnPopupOverlayIsOpenChanged(DependencyObject sender, DependencyProperty dp)
        {
            this.UpdateBackgroundBlur();
        }

        private void UpdateBackgroundBlur()
        {
            bool isBlurActive = this.PopupOverlay.IsOpen || this.LoadingOverlay.IsActive;
            double blueAmount = isBlurActive ? 3 : 0;
            
            this.Content.Blur(blueAmount, duration:200)?.Start();
        }
    }
}
