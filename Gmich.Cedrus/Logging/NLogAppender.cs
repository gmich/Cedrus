using System;

namespace Gmich.Cedrus.Logging
{
    internal class NLogAppender : IAppender
    {
        private NLogAuditor commonLogger;

        public LoggingContext LoggingContext { get; }

        public NLogAppender(LoggingContext loggingContext)
        {
            LoggingContext = loggingContext;
            commonLogger = new NLogAuditor(LoggingContext, "Gmich.Cedrus");
        }

        public IAppender this[Type logger]
        {
            get
            {
                commonLogger = new NLogAuditor(LoggingContext, logger.FullName);
                return this;
            }
        }
        public IAppender this[string logger]
        {
            get
            {
                commonLogger = new NLogAuditor(LoggingContext, logger);
                return this;
            }
        }

        public void Trace(string message)
        {
            commonLogger.Trace(message);
        }

        public void Trace(string message, params object[] args)
        {
            commonLogger.Trace(message, args);
        }

        public void Info(string message)
        {
            commonLogger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            commonLogger.Info(message, args);
        }

        public void Debug(string message)
        {
            commonLogger.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            commonLogger.Debug(message, args);
        }

        public void Warn(string message)
        {
            commonLogger.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            commonLogger.Warn(message, args);
        }

        public void Error(string message)
        {
            commonLogger.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            commonLogger.Error(message, args);
        }

        public void Fatal(string message)
        {
            commonLogger.Fatal(message);
        }

        public void Fatal(string message, params object[] args)
        {
            commonLogger.Fatal(message, args);
        }

    }
}