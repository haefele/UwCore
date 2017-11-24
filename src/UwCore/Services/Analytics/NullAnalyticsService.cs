using System;
using System.Collections.Generic;

namespace UwCore.Services.Analytics
{
    public class NullAnalyticsService : IAnalyticsService
    {
        public void TrackEvent(string eventName, IDictionary<string, string> properties = null)
        {
        }

        public void TrackException(Exception exception)
        {
        }
    }
}