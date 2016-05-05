using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace UwCore.Services.Navigation
{
    public interface INavigationService
    {
        IAdvancedNavigationService Advanced { get; }

        NavigateHelper<T> For<T>();
        void ClearBackStack();
    }

    public interface IAdvancedNavigationService
    {
        void Navigate(Type viewModelType, Dictionary<string, object> parameter = null);
    }
}