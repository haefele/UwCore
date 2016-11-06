using Windows.UI.Xaml;
using Caliburn.Micro;
using ReactiveUI;
using UwCore.Hamburger;

namespace UwCore.Application
{
    public interface IApplication
    {
        object HeaderDetailsViewModel { get; set; }

        ElementTheme Theme { get; set; }

        ApplicationMode CurrentMode { get; set; }

        ReactiveList<HamburgerItem> Actions { get; }
        ReactiveList<HamburgerItem> SecondaryActions { get; } 
    }
}