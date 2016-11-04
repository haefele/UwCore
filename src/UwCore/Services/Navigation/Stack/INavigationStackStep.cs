using System;

namespace UwCore.Services.Navigation
{
    public interface INavigationStackStep
    {
        bool CanGoBack();
        void GoBack();

        event EventHandler Changed;
    }
}