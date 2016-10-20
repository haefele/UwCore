using Windows.UI.Xaml.Controls;
using UwCore.Services.Navigation;

namespace UwCore.Hamburger
{
    public abstract class HamburgerItem
    {
        public HamburgerItem(string label, Symbol symbol, object tag = null)
        {
            this.Label = label;
            this.Symbol = symbol;
            this.Tag = tag;
        }
        
        public Symbol Symbol { get; }
        public string Label { get; }
        public object Tag { get; }

        public abstract void Execute();
    }
}