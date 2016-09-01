using System;
using System.Collections.Generic;

namespace UwCore.Services.Navigation
{
    public interface IAdvancedNavigationService
    {
        void Navigate(Type viewModelType, Dictionary<string, object> parameter = null, string context = null);
    }
}