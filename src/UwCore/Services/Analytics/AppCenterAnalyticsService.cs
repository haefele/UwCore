using System;
using System.Collections.Generic;
using Windows.Globalization;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using UwCore.Logging;
using AppAnalytics = Microsoft.AppCenter.Analytics.Analytics;

namespace UwCore.Services.Analytics
{
    public class AppCenterAnalyticsService : IAnalyticsService
    {
        public AppCenterAnalyticsService(string appSecret)
        {
            AppCenter.Start(appSecret, typeof(AppAnalytics), typeof(Crashes));
            AppCenter.SetCountryCode(new GeographicRegion().CodeTwoLetter);
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null)
        {
            AppAnalytics.TrackEvent(eventName, properties);
        }

        public void TrackException(Exception exception)
        {
            var logs = string.Join(Environment.NewLine, InMemoryLogMessages.GetLogs());
            var logsAttachment = ErrorAttachmentLog.AttachmentWithText(logs, "Logs.txt");

            Crashes.TrackError(exception, attachments: logsAttachment);
        }
    }
}