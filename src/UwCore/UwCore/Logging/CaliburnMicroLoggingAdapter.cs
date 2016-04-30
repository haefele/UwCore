using System;
using Caliburn.Micro;

namespace UwCore.Logging
{
    public class CaliburnMicroLoggingAdapter : ILog
    {
        private readonly Logger _logger;

        public CaliburnMicroLoggingAdapter(Logger logger)
        {
            this._logger = logger;
        }

        public void Info(string format, params object[] args)
        {
            this._logger.Information(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            this._logger.Warning(format, args);
        }

        public void Error(Exception exception)
        {
            this._logger.Error(exception, string.Empty);
        }
    }
}