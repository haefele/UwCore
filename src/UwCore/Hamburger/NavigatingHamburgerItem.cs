using System;
using Windows.UI.Xaml.Controls;

namespace UwCore.Hamburger
{
    public class NavigatingHamburgerItem : HamburgerItem
    {
        public NavigatingHamburgerItem(string label, Symbol symbol, Type viewModelType, object parameter = null)
            : base(label, symbol)
        {
            this.ViewModelType = viewModelType;
            this.Parameter = parameter;
        }

        public Type ViewModelType { get; }
        public object Parameter { get; }
    }
}