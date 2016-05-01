using System;

namespace UwCore.Services.Navigation
{
    public interface INavigationService
    {
        void Navigate(Type viewModelType, object parameter = null);
        void ClearBackStack();
    }
}