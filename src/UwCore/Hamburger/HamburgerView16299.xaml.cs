using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using UwCore.Controls;

namespace UwCore.Hamburger
{
    public sealed partial class HamburgerView16299 : Page, IHamburgerView
    {
        public HamburgerViewModel ViewModel => this.DataContext as HamburgerViewModel;

        public HamburgerView16299()
        {
            this.InitializeComponent();

            // Draw into the title bar
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            // Remove the solid-colored backgrounds behind the caption controls and system back button
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // App title position
            CoreApplicationViewTitleBar titleBar2 = CoreApplication.GetCurrentView().TitleBar;
            titleBar2.LayoutMetricsChanged += this.TitleBar_LayoutMetricsChanged;
            this.NavigationView.DisplayModeChanged += this.NavigationViewOnDisplayModeChanged;
            this.NavigationView.RegisterPropertyChangedCallback(NavigationView.IsPaneOpenProperty, this.OnNavigationVIewIsOpenChanged);
            
            // App title text
            this.AppTitle.Text = Package.Current.DisplayName;
            
            // Blur background
            this.PopupOverlay.RegisterPropertyChangedCallback(PopupOverlay.IsOpenProperty, this.OnPopupOverlayIsOpenChanged);
            this.LoadingOverlay.RegisterPropertyChangedCallback(LoadingOverlay.IsActiveProperty, this.OnLoadingOverlayIsActiveChanged);
        }

        #region Navigation
        private void HamburgerView16299_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            this.ViewModel.PropertyChanged += this.ViewModelOnPropertyChanged;

            this.ViewModel.Actions.Changed.Subscribe(this.OnActionsChanged);
            this.ViewModel.SecondaryActions.Changed.Subscribe(this.OnSecondaryActionsChanged);

            this.UpdateNavigationItems();
            this.UpdateSelectedItem();
        }

        private void OnActionsChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.UpdateNavigationItems();
        }

        private void OnSecondaryActionsChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.UpdateNavigationItems();
        }
        
        private void UpdateNavigationItems()
        {
            this.NavigationView.MenuItems.Clear();

            foreach (var action in this.ViewModel.Actions)
            {
                this.NavigationView.MenuItems.Add(new NavigationViewItem
                {
                    Icon = new SymbolIcon { Symbol = action.Symbol },
                    Content = action.Label,
                    DataContext = action,
                    Tag = action
                });
            }

            if (this.ViewModel.SecondaryActions.Any())
                this.NavigationView.MenuItems.Add(new NavigationViewItemSeparator());

            foreach (var action in this.ViewModel.SecondaryActions)
            {
                this.NavigationView.MenuItems.Add(new NavigationViewItem
                {
                    Icon = new SymbolIcon { Symbol = action.Symbol },
                    Content = action.Label,
                    DataContext = action
                });
            }
        }

        private bool _updatingSelectedItemFromViewModel = false;

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.ViewModel.SelectedAction) ||
                e.PropertyName == nameof(this.ViewModel.SelectedSecondaryAction))
            {
                this.UpdateSelectedItem();
            }
        }

        private void UpdateSelectedItem()
        {
            var selectedAction = this.ViewModel.SelectedAction ?? this.ViewModel.SelectedSecondaryAction;

            this._updatingSelectedItemFromViewModel = true;

            this.NavigationView.SelectedItem = this.NavigationView.MenuItems
                .OfType<NavigationViewItemBase>()
                .FirstOrDefault(f => f.DataContext == selectedAction);

            this._updatingSelectedItemFromViewModel = false;
        }

        private void NavigationView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem == null || this._updatingSelectedItemFromViewModel)
                return;

            var item = (NavigationViewItemBase)args.SelectedItem;
            var action = (HamburgerItem)item.DataContext;

            this.ViewModel.SelectedAction = this.ViewModel.Actions.Contains(action)
                ? action
                : null;

            this.ViewModel.SelectedSecondaryAction = this.ViewModel.SecondaryActions.Contains(action)
                ? action
                : null;

            action.Execute();
        }
        #endregion

        #region App title position
        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            this.UpdateAppTitlePosition();
        }

        private void NavigationViewOnDisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            this.UpdateAppTitlePosition();
        }

        private void OnNavigationVIewIsOpenChanged(DependencyObject sender, DependencyProperty dp)
        {
            this.UpdateAppTitlePosition();
        }

        private void UpdateAppTitlePosition()
        {
            double inset = CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset;

            var isMini = this.NavigationView.DisplayMode == NavigationViewDisplayMode.Minimal;
            var isCompact = this.NavigationView.DisplayMode == NavigationViewDisplayMode.Compact;
            var expandedButNotOpen = this.NavigationView.DisplayMode == NavigationViewDisplayMode.Expanded && this.NavigationView.IsPaneOpen == false;

            if (inset == 0 && (isCompact || expandedButNotOpen))
                inset = 48;

            //If its inset, make it show BEHIND the navigation view
            Canvas.SetZIndex(this.AppTitle, isMini || isCompact || expandedButNotOpen ? -1 : 0);
            this.AppTitle.Margin = new Thickness(inset + 12, 8, 0, 0);
        }
        #endregion

        #region Background blur
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

                var backgroundTask = this.NavigationView.Blur(backgroundBlurAmount, duration: 200)?.StartAsync() ?? Task.CompletedTask;
                var popupTask = this.PopupOverlay.Blur(popupBlurAmount, duration: 200)?.StartAsync() ?? Task.CompletedTask;

                await Task.WhenAll(backgroundTask, popupTask);
            }
            finally
            {
                this._updateBackgroundLock.Release();
            }
        }
        #endregion

        #region Implementation of IHamburgerView
        PopupOverlay IHamburgerView.PopupOverlay => this.PopupOverlay;
        Frame IHamburgerView.ContentFrame => this.ContentFrame;
        LoadingOverlay IHamburgerView.LoadingOverlay => this.LoadingOverlay;
        #endregion
    }
}
