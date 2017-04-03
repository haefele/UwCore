using System;

namespace UwCore.Services.Clock
{
    public interface IClock
    {
        DateTimeOffset Now();
        DateTimeOffset NowUtc();
    }

    public class RealtimeClock : IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }

        public DateTimeOffset NowUtc()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}