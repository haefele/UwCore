using System;

namespace UwCore.Services.Navigation.Stack
{
    public interface INavigationStep
    {
        bool CanGoBack();
        void GoBack();

        event EventHandler<NavigationStepChangedEventArgs> Changed;
    }

    public class NavigationStepChangedEventArgs : EventArgs
    {
        public object ViewModel { get; }

        public NavigationStepChangedEventArgs(object viewModel)
        {
            this.ViewModel = viewModel;
        }
    }
}