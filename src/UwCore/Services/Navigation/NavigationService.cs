using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Microsoft.HockeyApp;
using UwCore.Extensions;

namespace UwCore.Services.Navigation
{
    public class NavigationService : INavigationService, IAdvancedNavigationService, INavigationStackStep
    {
        private readonly Frame _frame;

        private readonly IEventAggregator _eventAggregator;
        private readonly IHockeyClient _hockeyClient;
        
        public NavigationService(Frame frame, IEventAggregator eventAggregator, IHockeyClient hockeyClient, PopupNavigationService popupNavigationService)
        {
            this._frame = frame;
            this._frame.Navigating += this.FrameOnNavigating;
            this._frame.Navigated += this.FrameOnNavigated;
            
            this._eventAggregator = eventAggregator;
            this._hockeyClient = hockeyClient;

            this.Popup = popupNavigationService;
        }

        public IAdvancedNavigationService Advanced => this;
        public IPopupNavigationService Popup { get; }

        public NavigateHelper<T> For<T>()
        {
            return new NavigateHelper<T>(this.Navigate);
        }

        internal void ClearBackStack()
        {
            this._frame.BackStack.Clear();

            this.Navigated?.Invoke(this, EventArgs.Empty);
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
            
            var frameworkElement = (FrameworkElement)e.Content;
            this._eventAggregator.PublishOnCurrentThread(new NavigatedEvent(frameworkElement.DataContext));

            this.Navigated?.Invoke(this, EventArgs.Empty);
        }

        #region Implementation of INavigationStackStep
        bool INavigationStackStep.CanGoBack()
        {
            return this._frame.CanGoBack;
        }

        void INavigationStackStep.GoBack()
        {
            this._frame.GoBack();
        }

        private event EventHandler Navigated;

        event EventHandler INavigationStackStep.Changed
        {
            add { this.Navigated += value; }
            remove { this.Navigated -= value; }
        }
        #endregion
    }
}