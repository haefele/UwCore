using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;

namespace UwCore.Services.Navigation
{
    public class NavigationService : FrameAdapter, INavigationService, IAdvancedNavigationService
    {
        private readonly IEventAggregator _eventAggregator;

        public NavigationService(Frame frame, IEventAggregator eventAggregator, bool treatViewAsLoaded = false)
            : base(frame, treatViewAsLoaded)
        {
            this._eventAggregator = eventAggregator;
        }

        public IAdvancedNavigationService Advanced => this;

        void IAdvancedNavigationService.Navigate(Type viewModelType, object parameter)
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
        }

        private void UpdateAppViewBackButtonVisibility()
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = this.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}