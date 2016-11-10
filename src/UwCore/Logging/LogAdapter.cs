using System;
using Caliburn.Micro;
using UwCore.Common;
using UwCore.Extensions;

namespace UwCore.Logging
{
    internal class LogAdapter : ILog, Microsoft.HockeyApp.ILog
    {
        private readonly Type _type;

        public LogAdapter(Type type)
        {
            Guard.NotNull(type, nameof(type));

            this._type = type;
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
            return $"{DateTimeOffset.Now:O} | {level.ToUpper()} | {this._type.FullName} | {message}";
        }
    }
}