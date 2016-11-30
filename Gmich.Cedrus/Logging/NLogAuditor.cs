using NLog;
using System;
using System.Globalization;

namespace Gmich.Cedrus.Logging
{

    internal class NLogAuditor : IAuditor
    {
        private readonly LoggingContext logInfo;
        private readonly IFormatProvider format = CultureInfo.InvariantCulture;
        public Logger Logger { get; }

        public NLogAuditor(LoggingContext logInfo, string appenderRef)
        {
            this.Logger = LogManager.GetLogger(appenderRef);
            this.logInfo = logInfo;
        }

        private void Log(Logger logger, LogLevel level, string msg)
        {
            var eventInfo = new LogEventInfo(
                level,
                this.Logger.Name,
                msg);
            logger.Log(eventInfo);
        }

        private void Log(Logger logger, LogLevel level, IFormatProvider format, string msg, params object[] args)
        {
            var eventInfo = new LogEventInfo(
                level,
                this.Logger.Name,
                format,
                msg,
                args);
            logger.Log(eventInfo);
        }

        public void Trace(string message)
        {
            Log(Logger, LogLevel.Trace, message);
        }

        public void Trace(string message, params object[] args)
        {
            Log(Logger, LogLevel.Trace, format, message, args);
        }

        public void Info(string message)
        {
            Log(Logger, LogLevel.Info, message);
        }

        public void Info(string message, params object[] args)
        {
            Log(Logger, LogLevel.Info, format, message, args);
        }

        public void Debug(string message)
        {
            Log(Logger, LogLevel.Debug, message);
        }

        public void Debug(string message, params object[] args)
        {
            Log(Logger, LogLevel.Debug, format, message, args);
        }

        public void Warn(string message)
        {
            Log(Logger, LogLevel.Warn, message);
        }

        public void Warn(string message, params object[] args)
        {
            Log(Logger, LogLevel.Warn, format, message, args);
        }

        public void Error(string message)
        {
            Log(Logger, LogLevel.Error, message);
        }

        public void Error(string message, params object[] args)
        {
            Log(Logger, LogLevel.Error, format, message, args);
        }

        public void Fatal(string message)
        {
            Log(Logger, LogLevel.Fatal, message);
        }

        public void Fatal(string message, params object[] args)
        {
            Log(Logger, LogLevel.Fatal, format, message, args);
        }

    }
}
