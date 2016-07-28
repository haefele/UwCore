using System;
using System.Collections.Generic;

namespace UwCore.Services.Navigation
{
    public interface IAdvancedPopupNavigationService
    {
        void Navigate(Type viewModelType, Dictionary<string, object> parameter = null);
    }
}