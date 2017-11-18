using Windows.UI.Xaml.Controls;
using UwCore.Controls;

namespace UwCore.Hamburger
{
    public interface IHamburgerView
    {
        PopupOverlay PopupOverlay { get; }
        Frame ContentFrame { get; }
        LoadingOverlay LoadingOverlay { get; }
    }
}