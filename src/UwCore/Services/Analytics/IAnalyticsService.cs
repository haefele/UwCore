using System;
using System.Collections.Generic;
using Windows.System.UserProfile;

namespace UwCore.Services.Analytics
{
    public interface IAnalyticsService
    {
        void TrackEvent(string eventName, IDictionary<string, string> properties = null);
        void TrackException(Exception exception);
    }
}