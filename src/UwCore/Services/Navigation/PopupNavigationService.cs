using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Caliburn.Micro;
using UwCore.Controls;

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
        }

        public IAdvancedPopupNavigationService Advanced => this;

        public NavigateHelper<T> For<T>()
        {
            return new NavigateHelper<T>(this.Navigate);
        }

        public void Navigate(Type viewModelType, Dictionary<string, object> parameter = null)
        {
            var viewModel = IoC.GetInstance(viewModelType, null);
            this.InjectParameters(viewModel, parameter);
            
            ScreenExtensions.TryActivate(viewModel);
            
            View.SetModel(this._popupOverlay, viewModel);
            this._popupOverlay.IsOpen = true;

            this._parent.UpdateAppViewBackButtonVisibility();
        }

        internal bool IsOpen()
        {
            return this._popupOverlay.IsOpen;
        }

        internal void Close()
        {
            var content = this._popupOverlay.Content as FrameworkElement;
            ScreenExtensions.TryDeactivate(content?.DataContext, true);

            this._popupOverlay.Content = null;
            this._popupOverlay.IsOpen = false;

            this._parent.UpdateAppViewBackButtonVisibility();
        }

        private void InjectParameters(object viewModel, Dictionary<string, object> parameter)
        {
            foreach (var pair in parameter)
            {
                var property = viewModel.GetType().GetPropertyCaseInsensitive(pair.Key);
                property?.SetValue(viewModel, MessageBinder.CoerceValue(property.PropertyType, pair.Value, null));
            }
        }
    }
}