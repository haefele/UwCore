using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Hamburger
{
    public class NavigatingHamburgerItem : HamburgerItem
    {
        public NavigatingHamburgerItem(string label, Symbol symbol, Type viewModelType, Dictionary<string, object> parameter = null)
            : base(label, symbol)
        {
            this.ViewModelType = viewModelType;
            this.Parameter = parameter;
        }

        public Type ViewModelType { get; }
        public Dictionary<string, object> Parameter { get; }

        public T TryGetParameterValue<T>(string name)
        {
            if (this.Parameter == null)
                return default(T);

            object value;
            if (this.Parameter.TryGetValue(name, out value) == false)
                return default(T);

            if (value is T == false)
                return default(T);

            return (T) value;
        }

        public override void Execute()
        {
            var navigationService = IoC.Get<INavigationService>();
            navigationService.Advanced.Navigate(this.ViewModelType, this.Parameter);
        }
    }
}