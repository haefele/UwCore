using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Caliburn.Micro;
using Microsoft.Toolkit.Uwp.UI.Animations;
using UwCore.Controls;
using PopupOverlayClass = UwCore.Controls.PopupOverlay;

namespace UwCore.Hamburger
{
    public sealed partial class HamburgerView : Page, IHamburgerView
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
        
        private readonly SemaphoreSlim _updateBackgroundLock = new SemaphoreSlim(1, 1);
        private async void UpdateBackgroundBlur()
        {
            await this._updateBackgroundLock.WaitAsync();
            try
            {
                bool isBackgroundBlurActive = this.PopupOverlay.IsOpen ^ this.LoadingOverlay.IsActive;
                double backgroundBlurAmount = isBackgroundBlurActive ? 3 : 0;

                bool isPopupBlurActive = this.PopupOverlay.IsOpen && this.LoadingOverlay.IsActive;
                double popupBlurAmount = isPopupBlurActive ? 3 : 0;
                
                var backgroundTask = this.Content.Blur(backgroundBlurAmount, duration: 200)?.StartAsync() ?? Task.CompletedTask;
                var popupTask = this.PopupOverlay.Blur(popupBlurAmount, duration: 200)?.StartAsync() ?? Task.CompletedTask;

                await Task.WhenAll(backgroundTask, popupTask);
            }
            finally
            {
                this._updateBackgroundLock.Release();
            }
        }

        #region Implementation of IHamburgerView
        PopupOverlayClass IHamburgerView.PopupOverlay => this.PopupOverlay;
        Frame IHamburgerView.ContentFrame => this.ContentFrame;
        LoadingOverlay IHamburgerView.LoadingOverlay => this.LoadingOverlay;
        #endregion
    }
}
