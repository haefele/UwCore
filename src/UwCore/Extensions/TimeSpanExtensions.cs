using System;

namespace UwCore.Extensions
{
    public static class TimeSpanExtensions
    {
        public static DateTime ToDateTime(this TimeSpan self)
        {
            return new DateTime(1, 1, 1).Add(self);
        }

        public static TimeSpan TrimMilliseconds(this TimeSpan self)
        {
            return self.Add(TimeSpan.FromMilliseconds(-self.Milliseconds));
        }
    }
}