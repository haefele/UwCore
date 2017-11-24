using System;
using System.Collections.Generic;
using Windows.Globalization;
using Microsoft.AppCenter;

namespace UwCore.Services.Analytics
{
    public class AppCenterAnalyticsService : IAnalyticsService
    {
        public AppCenterAnalyticsService(string appSecret, params Type[] services)
        {
            AppCenter.Start(appSecret, services);
            AppCenter.SetCountryCode(new GeographicRegion().CodeTwoLetter);
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName, properties);
        }

        public void TrackException(Exception exception)
        {
            //TODO: Implement
        }
    }
}