using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Hamburger
{
    public class NavigatingHamburgerItem : HamburgerItem
    {
        public NavigatingHamburgerItem(string label, Symbol symbol, Type viewModelType, Dictionary<string, object> parameters = null, object tag = null)
            : base(label, symbol, tag)
        {
            this.ViewModelType = viewModelType;
            this.Parameters = parameters ?? new Dictionary<string, object>();
        }

        public Type ViewModelType { get; }
        public Dictionary<string, object> Parameters { get; }

        
        public void AddParameter<T>(Expression<Func<T, object>> propertySelector, object value)
        {
            if (value is ValueType || !object.ReferenceEquals(null, value))
            {
                this.Parameters[propertySelector.GetMemberInfo().Name] = value.ToString();
            }
        }
        public T TryGetParameterValue<T>(string name)
        {
            object value;
            if (this.Parameters.TryGetValue(name, out value) == false)
                return default(T);

            if (value is T)
                return (T)value;

            return (T)MessageBinder.CoerceValue(typeof(T), value, null);
        }

        public override void Execute()
        {
            var navigationService = IoC.Get<INavigationService>();
            navigationService.Advanced.Navigate(this.ViewModelType, this.Parameters);
        }
    }
}