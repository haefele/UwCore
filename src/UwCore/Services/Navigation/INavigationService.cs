using System.Linq;

namespace UwCore.Services.Navigation
{
    public interface INavigationService
    {
        IAdvancedNavigationService Advanced { get; }

        IPopupNavigationService Popup { get; }

        NavigateHelper<T> For<T>();
    }
}