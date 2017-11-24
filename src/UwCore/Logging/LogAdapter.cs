using System;
using Caliburn.Micro;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.Clock;

namespace UwCore.Logging
{
    internal class LogAdapter : ILog
    {
        private readonly Type _type;
        private readonly IClock _clock;

        public LogAdapter(Type type, IClock clock)
        {
            Guard.NotNull(type, nameof(type));
            Guard.NotNull(clock, nameof(clock));

            this._type = type;
            this._clock = clock;
        }

        public void Info(string format, params object[] args)
        {
            var log = this.FormatLog("Info", format, args);
            InMemoryLogMessages.AddLog(log);
        }

        public void Warn(string format, params object[] args)
        {
            var log = this.FormatLog("Warn", format, args);
            InMemoryLogMessages.AddLog(log);
        }

        public void Error(Exception exception)
        {
            var log = this.FormatLog("Error", exception);
            InMemoryLogMessages.AddLog(log);
        }

        private string FormatLog(string level, string format, params object[] args)
        {
            return this.FormatLog(level, string.Format(format, args));
        }
        private string FormatLog(string level, Exception exception)
        {
            return this.FormatLog(level, exception.GetFullMessage());
        }
        private string FormatLog(string level, string message)
        {
            return $"{this._clock.Now():O} | {level.ToUpper()} | {this._type.FullName} | {message}";
        }
    }
}