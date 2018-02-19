using Windows.UI.Xaml.Controls;
using UwCore.Controls;

namespace UwCore.Hamburger
{
    public interface IHamburgerView
    {
        HamburgerViewModel ViewModel { get; }

        PopupOverlay PopupOverlay { get; }
        Frame ContentFrame { get; }
        LoadingOverlay LoadingOverlay { get; }
    }
}