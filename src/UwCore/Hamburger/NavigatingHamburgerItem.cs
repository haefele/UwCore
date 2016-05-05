using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

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
    }
}