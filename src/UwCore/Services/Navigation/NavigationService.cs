using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Microsoft.HockeyApp;
using UwCore.Controls;
using UwCore.Extensions;

namespace UwCore.Services.Navigation
{
    public class NavigationService : INavigationService, IAdvancedNavigationService
    {
        private readonly Frame _frame;

        private readonly IEventAggregator _eventAggregator;
        private readonly IHockeyClient _hockeyClient;

        private readonly PopupNavigationService _popupNavigationService;

        public NavigationService(Frame frame, PopupOverlay popupOverlay, IEventAggregator eventAggregator, IHockeyClient hockeyClient)
        {
            this._frame = frame;
            this._frame.Navigating += this.FrameOnNavigating;
            this._frame.Navigated += this.FrameOnNavigated;


            var navigationManager = SystemNavigationManager.GetForCurrentView();
            navigationManager.BackRequested += (s, e) =>
            {
                this.OnBackRequested(e);

                if (e.Handled)
                    return;

                if (this._frame.CanGoBack)
                {
                    e.Handled = true;
                    this._frame.GoBack();
                }
            };

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

        internal void ClearBackStack()
        {
            this._frame.BackStack.Clear();
            this.UpdateAppViewBackButtonVisibility();
        }

        public void Navigate(Type viewModelType, Dictionary<string, object> parameter, string context)
        {
            Type sourcePageType = ViewLocator.LocateTypeForModelType(viewModelType, null, context);

            if (sourcePageType == null)
                throw new InvalidOperationException($"No view was found for {viewModelType.FullName}. See the log for searched views.");

            this._frame.Navigate(sourcePageType, parameter);
        }

        private void FrameOnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            var view = this._frame.Content as FrameworkElement;

            if (view == null)
                return;

            var guard = view.DataContext as IGuardClose;

            if (guard != null)
            {
                var shouldCancel = false;
                var runningAsync = true;

                guard.CanClose(result =>
                {
                    runningAsync = false;
                    shouldCancel = !result;
                });

                if (runningAsync)
                    throw new NotSupportedException("Async CanClose is not supported.");

                e.Cancel = shouldCancel;

                if (e.Cancel)
                    return;
            }
            
            ScreenExtensions.TryDeactivate(view.DataContext, false);
        }

        private void FrameOnNavigated(object sender, NavigationEventArgs e)
        {
            ViewLocator.InitializeComponent(e.Content);

            var viewModel = ViewModelLocator.LocateForView(e.Content);

            if (viewModel == null)
                return;

            viewModel.InjectValues(e.Parameter as IDictionary<string, object>);
            ViewModelBinder.Bind(viewModel, e.Content as DependencyObject, null);

            ScreenExtensions.TryActivate(viewModel);

            this.UpdateAppViewBackButtonVisibility();

            if (this._popupNavigationService.IsOpen())
            {
                this._popupNavigationService.Close();
            }

            var frameworkElement = (FrameworkElement)e.Content;
            this._eventAggregator.PublishOnCurrentThread(new NavigatedEvent(frameworkElement.DataContext));

            this._hockeyClient.TrackEvent("Navigated", new Dictionary<string, string> { ["ViewModel"] = frameworkElement.DataContext.GetType().Name });
        }

        private void OnBackRequested(BackRequestedEventArgs e)
        {
            if (this._popupNavigationService.IsOpen())
            {
                this._popupNavigationService.Close();
                e.Handled = true;
            }
        }

        internal void UpdateAppViewBackButtonVisibility()
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = this._frame.CanGoBack || this._popupNavigationService.IsOpen() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}