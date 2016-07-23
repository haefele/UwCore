using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Microsoft.HockeyApp;

namespace UwCore.Services.Navigation
{
    public class NavigationService : FrameAdapter, INavigationService, IAdvancedNavigationService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IHockeyClient _hockeyClient;

        public NavigationService(Frame frame, IEventAggregator eventAggregator, IHockeyClient hockeyClient, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
            this._eventAggregator = eventAggregator;
            this._hockeyClient = hockeyClient;
        }

        public IAdvancedNavigationService Advanced => this;

        void IAdvancedNavigationService.Navigate(Type viewModelType, Dictionary<string, object> parameter)
        {
            this.NavigateToViewModel(viewModelType, parameter);
        }

        public NavigateHelper<T> For<T>()
        {
            return new NavigateHelper<T>().AttachTo(this);
        }

        public void ClearBackStack()
        {
            this.BackStack.Clear();
            this.UpdateAppViewBackButtonVisibility();
        }
        
        protected override void OnNavigated(object sender, NavigationEventArgs e)
        {
            base.OnNavigated(sender, e);

            this.UpdateAppViewBackButtonVisibility();

            var frameworkElement = (FrameworkElement)e.Content;
            this._eventAggregator.PublishOnCurrentThread(new NavigatedEvent(frameworkElement.DataContext));

            this._hockeyClient.TrackEvent("Navigated", new Dictionary<string, string> { ["ViewModel"] = frameworkElement.DataContext.GetType().Name });
        }

        private void UpdateAppViewBackButtonVisibility()
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = this.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}