using Windows.UI.Xaml.Controls;

namespace UwCore.Hamburger
{
    public abstract class HamburgerItem
    {
        public HamburgerItem(string label, Symbol symbol)
        {
            this.Label = label;
            this.Symbol = symbol;
        }
        
        public Symbol Symbol { get; }
        public string Label { get; }
    }
}