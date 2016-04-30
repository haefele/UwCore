using System;
using Windows.UI.Xaml.Controls;

namespace UwCore.Hamburger
{
    public class NavigatingHamburgerItem : HamburgerItem
    {
        public NavigatingHamburgerItem(string label, Symbol symbol, Type viewModelType)
            : base(label, symbol)
        {
            this.ViewModelType = viewModelType;
        }

        public Type ViewModelType { get; }
    }
}