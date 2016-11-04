using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Caliburn.Micro;
using UwCore.Controls;
using UwCore.Extensions;

namespace UwCore.Services.Navigation
{
    public class PopupNavigationService : IPopupNavigationService, IAdvancedPopupNavigationService
    {
        private readonly NavigationService _parent;
        private readonly PopupOverlay _popupOverlay;

        public PopupNavigationService(NavigationService parent, PopupOverlay popupOverlay)
        {
            this._parent = parent;
            this._popupOverlay = popupOverlay;

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
            viewModel.InjectValues(parameter);
            
            ScreenExtensions.TryActivate(viewModel);
            
            View.SetContext(this._popupOverlay, context);
            View.SetModel(this._popupOverlay, viewModel);

            this._popupOverlay.Show();

            this._parent.UpdateAppViewBackButtonVisibility();
        }

        internal bool IsOpen()
        {
            return this._popupOverlay.IsOpen;
        }

        internal void Close()
        {
            this._popupOverlay.Close();
        }

        private void PopupOverlayOnClosing(object sender, PopupOverlayClosingEventArgs e)
        {
            var content = this._popupOverlay.Content as FrameworkElement;
            var viewModel = content?.DataContext;
            var guardClose = viewModel as IGuardClose;

            if (guardClose != null)
            {
                var shouldCancel = false;
                var runningAsync = true;
            
                guardClose.CanClose(result => 
                {
                    runningAsync = false;
                    shouldCancel = !result;
                });

                if (runningAsync)
                    throw new NotSupportedException("Async CanClose is not supported.");
                
                e.Cancel = shouldCancel;
            }
        }

        private void PopupOverlayOnClosed(object sender, EventArgs e)
        {
            var content = this._popupOverlay.Content as FrameworkElement;
            ScreenExtensions.TryDeactivate(content?.DataContext, true);

            this._popupOverlay.Content = null;

            this._parent.UpdateAppViewBackButtonVisibility();
        }
    }
}