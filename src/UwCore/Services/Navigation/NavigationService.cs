using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Microsoft.HockeyApp;
using UwCore.Controls;

namespace UwCore.Services.Navigation
{
    public class NavigationService : FrameAdapter, INavigationService, IAdvancedNavigationService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IHockeyClient _hockeyClient;

        private readonly PopupNavigationService _popupNavigationService;

        public NavigationService(Frame frame, PopupOverlay popupOverlay, IEventAggregator eventAggregator, IHockeyClient hockeyClient, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
            this._eventAggregator = eventAggregator;
            this._hockeyClient = hockeyClient;

            this._popupNavigationService = new PopupNavigationService(this, popupOverlay);
        }

        public IAdvancedNavigationService Advanced => this;
        public IPopupNavigationService Popup => this._popupNavigationService;

        public NavigateHelper<T> For<T>()
        {
            return new NavigateHelper<T>(this.Navigate);
        }

        public void ClearBackStack()
        {
            this.BackStack.Clear();
            this.UpdateAppViewBackButtonVisibility();
        }

        public void Navigate(Type viewModelType, Dictionary<string, object> parameter)
        {
            this.NavigateToViewModel(viewModelType, parameter);
        }

        protected override void OnNavigated(object sender, NavigationEventArgs e)
        {
            base.OnNavigated(sender, e);

            this.UpdateAppViewBackButtonVisibility();

            var frameworkElement = (FrameworkElement)e.Content;
            this._eventAggregator.PublishOnCurrentThread(new NavigatedEvent(frameworkElement.DataContext));

            this._hockeyClient.TrackEvent("Navigated", new Dictionary<string, string> { ["ViewModel"] = frameworkElement.DataContext.GetType().Name });
        }

        protected override void OnBackRequested(BackRequestedEventArgs e)
        {
            base.OnBackRequested(e);

            if (this._popupNavigationService.IsOpen())
            {
                this._popupNavigationService.Close();
                e.Handled = true;
            }
        }

        internal void UpdateAppViewBackButtonVisibility()
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = this.CanGoBack || this._popupNavigationService.IsOpen() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}