using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using UwCore.Hamburger;

namespace UwCore.Application
{
    public interface IApplication
    {
        ApplicationMode CurrentMode { get; set; }

        ReactiveObservableCollection<HamburgerItem> Actions { get; }
        ReactiveObservableCollection<HamburgerItem> SecondaryActions { get; } 
    }
}