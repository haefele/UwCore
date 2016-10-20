using System;
using Windows.UI.Xaml.Controls;

namespace UwCore.Hamburger
{
    public class ClickableHamburgerItem : HamburgerItem
    {
        public ClickableHamburgerItem(string label, Symbol symbol, Action action, object tag = null)
            : base(label, symbol, tag)
        {
            this.Action = action;
        }

        public Action Action { get; }

        public override void Execute()
        {
            this.Action();
        }
    }
}