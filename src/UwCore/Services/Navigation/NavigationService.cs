using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using UwCore.Extensions;
using UwCore.Helpers;
using UwCore.Services.Navigation.Stack;

namespace UwCore.Services.Navigation
{
    public class NavigationService : INavigationService, IAdvancedNavigationService, INavigationStep
    {
        private readonly Frame _frame;

        private readonly IEventAggregator _eventAggregator;
        
        public NavigationService(Frame frame, IEventAggregator eventAggregator, PopupNavigationService popupNavigationService)
        {
            this._frame = frame;
            this._frame.Navigating += this.FrameOnNavigating;
            this._frame.Navigated += this.FrameOnNavigated;
            
            this._eventAggregator = eventAggregator;

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

            this.Changed?.Invoke(this, new NavigationStepChangedEventArgs(null));
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

            bool cancel = CaliburnMicroHelper.TryGuardClose(view.DataContext);
            if (cancel)
            {
                e.Cancel = true;
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

            ParametersHelper.InjectParameter(viewModel, e.Parameter as IDictionary<string, object>);
            ViewModelBinder.Bind(viewModel, e.Content as DependencyObject, null);
            
            ScreenExtensions.TryActivate(viewModel);
            
            this.Changed?.Invoke(this, new NavigationStepChangedEventArgs(viewModel));
        }

        #region Implementation of INavigationStackStep
        bool INavigationStep.CanGoBack()
        {
            return this._frame.CanGoBack;
        }

        void INavigationStep.GoBack()
        {
            this._frame.GoBack();
        }

        private event EventHandler<NavigationStepChangedEventArgs> Changed;

        event EventHandler<NavigationStepChangedEventArgs> INavigationStep.Changed
        {
            add { this.Changed += value; }
            remove { this.Changed -= value; }
        }
        #endregion
    }
}