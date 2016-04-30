using Caliburn.Micro;
using UwCore.Hamburger;

namespace UwCore.Application
{
    public interface IApplication
    {
        ApplicationMode CurrentMode { get; set; }

        BindableCollection<HamburgerItem> Actions { get; } 
        BindableCollection<HamburgerItem> SecondaryActions { get; } 
    }
}