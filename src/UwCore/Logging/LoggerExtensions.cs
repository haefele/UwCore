using System;

namespace UwCore.Logging
{
    public static class LoggerExtensions
    {
        public static void Debug(this Logger logger, Func<string> message)
        {
            if (logger.IsDebugEnabled)
                logger.Debug(message());
        }
        public static void Debug(this Logger logger, Exception exception, Func<string> message)
        {
            if (logger.IsDebugEnabled)
                logger.Debug(exception, message());
        }
        public static void Information(this Logger logger, Func<string> message)
        {
            if (logger.IsInformationEnabled)
                logger.Information(message());
        }
        public static void Information(this Logger logger, Exception exception, Func<string> message)
        {
            if (logger.IsInformationEnabled)
                logger.Information(exception, message());
        }
        public static void Warning(this Logger logger, Func<string> message)
        {
            if (logger.IsWarningEnabled)
                logger.Warning(message());
        }
        public static void Warning(this Logger logger, Exception exception, Func<string> message)
        {
            if (logger.IsWarningEnabled)
                logger.Warning(exception, message());
        }
        public static void Error(this Logger logger, Func<string> message)
        {
            if (logger.IsErrorEnabled)
                logger.Error(message());
        }
        public static void Error(this Logger logger, Exception exception, Func<string> message)
        {
            if (logger.IsErrorEnabled)
                logger.Error(exception, message());
        }
        public static void Fatal(this Logger logger, Func<string> message)
        {
            if (logger.IsFatalEnabled)
                logger.Fatal(message());
        }
        public static void Fatal(this Logger logger, Exception exception, Func<string> message)
        {
            if (logger.IsFatalEnabled)
                logger.Fatal(exception, message());
        }
    }
}