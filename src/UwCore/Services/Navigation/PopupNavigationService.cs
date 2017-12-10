using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Caliburn.Micro;
using UwCore.Controls;
using UwCore.Extensions;
using UwCore.Helpers;
using UwCore.Services.Navigation.Stack;

namespace UwCore.Services.Navigation
{
    public class PopupNavigationService : IPopupNavigationService, IAdvancedPopupNavigationService, INavigationStep
    {
        private readonly PopupOverlay _popupOverlay;
        private readonly INavigationStack _navigationStack;

        public PopupNavigationService(PopupOverlay popupOverlay, INavigationStack navigationStack)
        {
            this._popupOverlay = popupOverlay;
            this._navigationStack = navigationStack;

            this._popupOverlay.Closing += this.PopupOverlayOnClosing;
            this._popupOverlay.Closed += this.PopupOverlayOnClosed;
        }

        public IAdvancedPopupNavigationService Advanced => this;

        public NavigateHelper<T> For<T>()
        {
            return new NavigateHelper<T>(this.Navigate);
        }

        public void Navigate(Type viewModelType, Dictionary<string, object> parameter = null, string context = null)
        {
            var viewModel = IoC.GetInstance(viewModelType, null);
            ParametersHelper.InjectParameter(viewModel, parameter);
            
            if (this._popupOverlay.Show())
            {
                ScreenExtensions.TryActivate(viewModel);
            
                View.SetContext(this._popupOverlay, context);
                View.SetModel(this._popupOverlay, viewModel);

                this._navigationStack.AddStep(this);
                this.Changed?.Invoke(this, new NavigationStepChangedEventArgs(viewModel));
            }
        }

        private void PopupOverlayOnClosing(object sender, PopupOverlayClosingEventArgs e)
        {
            var content = this._popupOverlay.Content as FrameworkElement;
            var viewModel = content?.DataContext;

            var cancel = CaliburnMicroHelper.TryGuardClose(viewModel);
            e.Cancel = cancel;
        }

        private void PopupOverlayOnClosed(object sender, EventArgs e)
        {
            var content = this._popupOverlay.Content as FrameworkElement;
            ScreenExtensions.TryDeactivate(content?.DataContext, true);

            // Leave the content in there to allow a smooth animation
            //this._popupOverlay.Content = null;

            this.Changed?.Invoke(this, new NavigationStepChangedEventArgs(null));
            this._navigationStack.RemoveStep(this);
        }

        #region Implementation of INavigationStackStep
        bool INavigationStep.CanGoBack()
        {
            return this._popupOverlay.IsOpen;
        }

        void INavigationStep.GoBack()
        {
            this._popupOverlay.Close();
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