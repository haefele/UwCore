using Windows.UI.Xaml;
using Caliburn.Micro;
using ReactiveUI;
using UwCore.Hamburger;
using DynamicData.Binding;

namespace UwCore.Application
{
    public interface IShell
    {
        object HeaderDetailsViewModel { get; set; }

        ElementTheme Theme { get; set; }

        ShellMode CurrentMode { get; set; }

        ObservableCollectionExtended<HamburgerItem> Actions { get; }
        ObservableCollectionExtended<HamburgerItem> SecondaryActions { get; } 
    }
}