using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Markup;
using Caliburn.Micro;
using Microsoft.Toolkit.Uwp.UI.Animations;
using UwCore.Controls;
using UwCore.Extensions;

namespace UwCore.Hamburger
{
    public sealed partial class HamburgerView16299 : Page, IHamburgerView
    {
        private readonly DataTemplate _navigationItemContentDataTemplate;

        public HamburgerViewModel ViewModel => this.DataContext as HamburgerViewModel;

        public HamburgerView16299()
        {
            this.InitializeComponent();

            this._navigationItemContentDataTemplate = (DataTemplate) XamlReader.Load("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TextBlock Text=\"{Binding Label, Mode=OneWay}\" /></DataTemplate>");

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
            this.NavigationView.DisplayModeChanged += this.NavigationViewOnDisplayModeChangedForAutoExpand;
            this.NavigationView.RegisterPropertyChangedCallback(NavigationView.IsPaneOpenProperty, this.OnNavigationVIewIsOpenChanged);
            
            // App title text
            this.AppTitle.Text = Package.Current.DisplayName;
            
            // Blur background
            this.PopupOverlay.RegisterPropertyChangedCallback(PopupOverlay.IsOpenProperty, this.OnPopupOverlayIsOpenChanged);
            this.LoadingOverlay.RegisterPropertyChangedCallback(LoadingOverlay.IsActiveProperty, this.OnLoadingOverlayIsActiveChanged);

            //Theme changes
            var settings = new UISettings();
            settings.ColorValuesChanged += (s, e) => this.UpdateTitleBarButtonForegroundColors();
        }

        #region ViewModel integration
        private HamburgerViewModel _previousViewModel;

        private void HamburgerView16299_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (this._previousViewModel == this.ViewModel)
                return;

            this._previousViewModel = this.ViewModel;

            this.ViewModel.PropertyChanged += this.ViewModelOnPropertyChanged;

            this.ViewModel.Actions.Changed.Subscribe(this.OnActionsChanged);
            this.ViewModel.SecondaryActions.Changed.Subscribe(this.OnSecondaryActionsChanged);

            this.UpdateNavigationItems();
            this.UpdateSelectedItem();
            this.UpdateTitleBarButtonForegroundColors();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.ViewModel.SelectedAction) ||
                e.PropertyName == nameof(this.ViewModel.SelectedSecondaryAction))
            {
                this.UpdateSelectedItem();
            }

            if (e.PropertyName == nameof(this.ViewModel.Theme))
            {
                this.UpdateTitleBarButtonForegroundColors();
            }
        }
        #endregion

        #region Navigation
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
            var newNavigation = new List<object>();

            foreach (var action in this.ViewModel.Actions)
            {
                var item = new NavigationViewItem
                {
                    Icon = new SymbolIcon { Symbol = action.Symbol },
                    Content = action,
                    ContentTemplate = this._navigationItemContentDataTemplate,
                    DataContext = action,
                };
                ToolTipService.SetToolTip(item, action.Label);

                newNavigation.Add(item);
            }

            if (this.ViewModel.SecondaryActions.Any())
                newNavigation.Add(new NavigationViewItemSeparator());

            foreach (var action in this.ViewModel.SecondaryActions)
            {
                var item = new NavigationViewItem
                {
                    Icon = new SymbolIcon { Symbol = action.Symbol },
                    Content = action,
                    ContentTemplate = this._navigationItemContentDataTemplate,
                    DataContext = action
                };
                ToolTipService.SetToolTip(item, action.Label);

                newNavigation.Add(item);
            }

            if (this.NavigationView.MenuItems.SequenceEqual(newNavigation, new NavigationViewItemEqualityComparer()) == false)
            {
                this.NavigationView.MenuItems.Clear();
                this.NavigationView.MenuItems.AddRange(newNavigation);
            }
        }
        
        private void UpdateSelectedItem()
        {
            var selectedAction = this.ViewModel.SelectedAction ?? this.ViewModel.SelectedSecondaryAction;
            
            this.NavigationView.SelectedItem = this.NavigationView.MenuItems
                .OfType<NavigationViewItemBase>()
                .FirstOrDefault(f => f.DataContext == selectedAction);
        }

        private void NavigationView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem == null)
                return;

            var item = (NavigationViewItemBase)args.SelectedItem;
            var action = (HamburgerItem)item.DataContext;

            this.ViewModel.SelectedAction = this.ViewModel.Actions.Contains(action)
                ? action
                : null;

            this.ViewModel.SelectedSecondaryAction = this.ViewModel.SecondaryActions.Contains(action)
                ? action
                : null;
        }

        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = (HamburgerItem)args.InvokedItem;
            item.Execute();
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

        #region Window shell button foreground colors
        private void UpdateTitleBarButtonForegroundColors()
        {
            bool IsDarkThemeCurrently()
            {
                if (this.ViewModel == null)
                    return false; // Whatever, doesnt really matter because we will update again once we have a ViewModel

                if (this.ViewModel.Theme == ElementTheme.Default)
                {
                    var settings = new UISettings();
                    return settings.GetColorValue(UIColorType.Background) == Colors.Black;
                }

                return this.ViewModel.Theme == ElementTheme.Dark;
            }

            Execute.OnUIThread(() =>
            {
                Color color = IsDarkThemeCurrently()
                    ? Colors.White
                    : Colors.Black;

                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = color;
            });
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

        #region Auto expand navigation if wide enough
        private void NavigationViewOnDisplayModeChangedForAutoExpand(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
        {
            if (this.NavigationView.DisplayMode == NavigationViewDisplayMode.Expanded)
                this.NavigationView.IsPaneOpen = true;
        }
        #endregion

        #region Implementation of IHamburgerView
        PopupOverlay IHamburgerView.PopupOverlay => this.PopupOverlay;
        Frame IHamburgerView.ContentFrame => this.ContentFrame;
        LoadingOverlay IHamburgerView.LoadingOverlay => this.LoadingOverlay;
        #endregion

        #region Implementation
        private class NavigationViewItemEqualityComparer : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                if (x is NavigationViewItemSeparator && y is NavigationViewItemSeparator)
                    return true;

                if (x is NavigationViewItem xItem && y is NavigationViewItem yItem)
                {
                    var xHamburgerItem = xItem.DataContext as HamburgerItem;
                    var yHamburgerItem = yItem.DataContext as HamburgerItem;

                    return object.Equals(xHamburgerItem, yHamburgerItem);
                }

                return object.Equals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                if (obj is NavigationViewItemSeparator separator)
                    return 0;

                if (obj is NavigationViewItem item && item.DataContext is HamburgerItem hamburgerItem)
                    return hamburgerItem.GetHashCode();

                return obj.GetHashCode();
            }
        }
        #endregion
    }
}
