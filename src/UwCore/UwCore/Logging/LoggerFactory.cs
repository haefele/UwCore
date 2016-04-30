using System;
using System.IO;
using System.Threading.Tasks;
using MetroLog;
using MetroLog.Targets;

namespace UwCore.Logging
{
    public class LoggerFactory
    {
        private static readonly ILogManager LogManager;

        static LoggerFactory()
        {
            var config = new LoggingConfiguration();
            config.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            config.AddTarget(LogLevel.Trace, LogLevel.Fatal, new DebugTarget());

            LogManager = LogManagerFactory.CreateLogManager(config);
        }

        public static Logger GetLogger(Type type)
        {
            return new Logger(LogManager.GetLogger(type));
        }

        public static Logger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static Task<Stream> GetCompressedLogs()
        {
            return LogManager.GetCompressedLogs();
        }
    }
}