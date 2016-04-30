using System;
using Windows.UI.Xaml.Controls;

namespace UwCore.Hamburger
{
    public class ClickableHamburgerItem : HamburgerItem
    {
        public ClickableHamburgerItem(string label, Symbol symbol, Action action)
            : base(label, symbol)
        {
            this.Action = action;
        }

        public Action Action { get; }
    }
}